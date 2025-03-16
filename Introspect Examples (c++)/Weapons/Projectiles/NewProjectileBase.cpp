// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "Engine.h"
#include "GameEntities/Weapons/CanTakeDamage.h"
#include "GameEntities/Weapons/WeaponBase.h"
#include "NewProjectileBase.h"

ANewProjectileBase::ANewProjectileBase()
{
	PrimaryActorTick.bCanEverTick = false;

	// Mesh Component
	StaticMeshComponent = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Static Mesh"));
	StaticMeshComponent->SetupAttachment(RootComponent);
	//StaticMeshComponent->SetCollisionProfileName(TEXT("Projectile"));
	//StaticMeshComponent->SetCollisionResponseToAllChannels(ECollisionResponse::ECR_Overlap);
	StaticMeshComponent->SetCollisionResponseToChannel(ECollisionChannel::ECC_GameTraceChannel4, ECollisionResponse::ECR_Ignore);
	//StaticMeshComponent->SetCollisionObjectType(ECollisionChannel::ECC_WorldDynamic);

	// Set the mesh as the root component
	RootComponent = StaticMeshComponent;

	// Movement Component
	ProjectileMovementComponent = CreateDefaultSubobject<UProjectileMovementComponent>(TEXT("Projectile Movement"));
	ProjectileMovementComponent->SetUpdatedComponent(RootComponent);
}

void ANewProjectileBase::BeginPlay()
{
	Super::BeginPlay();

	iBaseDamage = iDamageToDeal;

	// Movement variables
	ProjectileMovementComponent->InitialSpeed = fTravelSpeed;
	ProjectileMovementComponent->MaxSpeed = fTravelSpeed;
	ProjectileMovementComponent->bRotationFollowsVelocity = true;
	ProjectileMovementComponent->ProjectileGravityScale = fGravityScale;

	// Bind the collision function to the mesh
	StaticMeshComponent->OnComponentBeginOverlap.AddDynamic(this, &ANewProjectileBase::OnMeshOverlap);
}

void ANewProjectileBase::Initialize(FVector Position, FVector Velocity, float LifeTime, TArray<TSubclassOf<AActor>> IgnoredClasses, TQueue<ANewProjectileBase*>* Return)
{
	SetActorLocation(Position);
	ProjectileMovementComponent->Velocity = Velocity.GetSafeNormal() * fTravelSpeed;
	ReturnQueue = Return;
	fLifetime = LifeTime;
	ClassesToIgnore = IgnoredClasses;

	StaticMeshComponent->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);

	// Clear damage multiplier
	iDamageToDeal = iBaseDamage;

	// Set timer so that OnLifetimeExired is called after the allotted time
	GetWorldTimerManager().SetTimer(LifetimeHandle, this, &ANewProjectileBase::OnLifetimeExpired, fLifetime, false);
}

void ANewProjectileBase::AddDamageMultiplier(float DamageMultiplier)
{
	iDamageToDeal *= DamageMultiplier;
}

void ANewProjectileBase::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
}

void ANewProjectileBase::OnHitActor(AActor * actor, const FHitResult & SweepResult)
{
	EHitType HitType = Cast<ICanTakeDamage>(actor)->Execute_ObjectTakeDamage(actor, iDamageToDeal, 1.0f, SweepResult, SourceName);
	Cast<AWeaponBase>(GetOwner())->HitShot(Cast<AWeaponBase>(GetOwner())->HitMarkerFromHitType(HitType));
	Cast<AWeaponBase>(GetOwner())->SpawnDamageNumberMarker(GetActorLocation(), iBaseDamage, HitType);

	// Clear the timer and destroy
	GetWorldTimerManager().ClearTimer(LifetimeHandle);
	OnLifetimeExpired();
}

void ANewProjectileBase::OnHitComponent(UPrimitiveComponent * component, const FHitResult & SweepResult)
{
	EHitType HitType = Cast<ICanTakeDamage>(component)->Execute_ObjectTakeDamage(component, iDamageToDeal, 1.0f, SweepResult, SourceName);
	Cast<AWeaponBase>(GetOwner())->HitShot(Cast<AWeaponBase>(GetOwner())->HitMarkerFromHitType(HitType));
	Cast<AWeaponBase>(GetOwner())->SpawnDamageNumberMarker(GetActorLocation(), iBaseDamage, HitType);
	
	// Clear the timer and destroy
	GetWorldTimerManager().ClearTimer(LifetimeHandle);
	OnLifetimeExpired();
}

void ANewProjectileBase::OnHitTerrain(const FHitResult &SweepResult)
{
	// Clear the timer and destroy
	GetWorldTimerManager().ClearTimer(LifetimeHandle);
	OnLifetimeExpired();
}

void ANewProjectileBase::ResetBullet()
{
	// Move to origin
	StaticMeshComponent->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	SetActorLocation(FVector(0.0f, 0.0f, -10000.0f));
	ProjectileMovementComponent->Velocity = FVector::ZeroVector;
}

void ANewProjectileBase::OnLifetimeExpired()
{	
	GetWorldTimerManager().ClearTimer(LifetimeHandle);

	// Play sound, animation, and particle effect
	UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), ParticleOnKill, GetActorLocation());
	UGameplayStatics::SpawnSoundAtLocation(GetWorld(), SoundOnKill, GetActorLocation());

	if (ReturnQueue)
	{
		// If so, return to the pool and move this bullet off screen
		ResetBullet();
		ReturnQueue->Enqueue(this);
	}
	else
	{
		Destroy();
	}
}

void ANewProjectileBase::SpawnDecal(FHitResult Hit)
{
	ADecalActor* decal = GetWorld()->SpawnActor<ADecalActor>(HitDecal, Hit.ImpactPoint, Hit.Normal.Rotation());
	if (decal)
	{
		decal->GetDecal()->AddRelativeRotation(FRotator(90.0f, 0.0f, 0.0f));
		decal->GetDecal()->AttachToComponent(Hit.GetComponent(), FAttachmentTransformRules(EAttachmentRule::KeepWorld, EAttachmentRule::KeepWorld, EAttachmentRule::KeepWorld, true), Hit.BoneName);
	}
}

void ANewProjectileBase::OnMeshOverlap(UPrimitiveComponent* OverlappedComp, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult &SweepResult)
{
	// Check for ignored actors
	bool b = true;
	for (TSubclassOf<AActor> a : ClassesToIgnore)
	{
		if (OtherActor->GetClass() == a)
		{
			GEngine->AddOnScreenDebugMessage(-1, 3.0f, FColor::Purple, "Hit: " + OtherActor->GetClass()->GetName());
			b = false;
			break;
		}
	}
	
	if (OtherActor != GetOwner() && b)
	{
		// Check if we hit the player
		AIntrospectCharacter* player = Cast<AIntrospectCharacter>(OtherActor);
		if (player && bCanDamagePlayer)
		{
			player->InflictDamage(iDamageToDeal);
		}
		else
		{
			ICanTakeDamage* component = Cast<ICanTakeDamage>(OtherComp);
			ICanTakeDamage* actor = Cast<ICanTakeDamage>(OtherActor);

			// Test to see if we hit a boss
			if (component)
			{
				OnHitComponent(OtherComp, SweepResult);

			}
			else if (actor)
			{
				OnHitActor(OtherActor, SweepResult);
			}
			// Otherwise we hit terrain
			else
			{
				OnHitTerrain(SweepResult);
			}
		}

		// Spawn decal
		SpawnDecal(SweepResult);
	}
}

TMap<ICanTakeDamage*, UObject*> ANewProjectileBase::GetDamageableObjectsInRange(UPrimitiveComponent* range)
{
	TMap<ICanTakeDamage*, UObject*> all;

	TSet<AActor*> a;
	range->GetOverlappingActors(a);
	TArray<UPrimitiveComponent*> b;
	range->GetOverlappingComponents(b);

	for (AActor* aa : a)
	{
		ICanTakeDamage* aaa = Cast<ICanTakeDamage>(aa);
		if (aaa)
		{
			all.Add(aaa, aa);
		}
	}

	for (UPrimitiveComponent* bb : b)
	{
		ICanTakeDamage* bbb = Cast<ICanTakeDamage>(bb);
		if (bbb)
		{
			all.Add(bbb, bb);
		}
	}

	return all;
}