// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "NewBulldogProjectile.h"

ANewBulldogProjectile::ANewBulldogProjectile() : Super()
{
	LazerParticle = CreateDefaultSubobject<UParticleSystemComponent>(TEXT("Lazer Particle"));
	LazerParticle->SetupAttachment(RootComponent);
	LazerParticle->Deactivate();
}

void ANewBulldogProjectile::Initialize(FVector Position, FVector Velocity, float LifeTime, TArray<TSubclassOf<AActor>> IgnoredClasses, TQueue<ANewProjectileBase*>* Return)
{
	Super::Initialize(Position, Velocity, LifeTime, IgnoredClasses, Return);

	LazerParticle->Activate();
}

void ANewBulldogProjectile::ResetBullet()
{
	Super::ResetBullet();

	LazerParticle->Deactivate();
}
