// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "GameEntities/Bosses/BossBase.h"
#include "IntrospectCharacter.h"
#include "NewProjectileBase.generated.h"

UCLASS()
class INTROSPECT_API ANewProjectileBase : public AActor
{
	GENERATED_BODY()
	
public:	

	// Methods

	ANewProjectileBase();
	virtual void Initialize(FVector Position, FVector Velocity, float LifeTime, TArray<TSubclassOf<AActor>> IgnoredClasses, TQueue<ANewProjectileBase*>* Return = nullptr);
	void AddDamageMultiplier(float DamageMultiplier);

	// Variables

	

protected:

	// Methods

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

	virtual void OnHitActor(AActor* actor, const FHitResult &SweepResult);
	virtual void OnHitComponent(UPrimitiveComponent* component, const FHitResult &SweepResult);
	virtual void OnHitTerrain(const FHitResult &SweepResult);
	virtual void ResetBullet();
	virtual void OnLifetimeExpired();

	UFUNCTION()
		void OnMeshOverlap(UPrimitiveComponent* OverlappedComp, AActor* OtherActor,
			UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult &SweepResult);

	TMap<ICanTakeDamage*, UObject*> GetDamageableObjectsInRange(UPrimitiveComponent* range);

	// Variables

	UPROPERTY(EditAnywhere, Category = "Components")
		UProjectileMovementComponent* ProjectileMovementComponent;
	UPROPERTY(EditAnywhere, Category = "Components")
		UStaticMeshComponent* StaticMeshComponent;

	UPROPERTY(EditAnywhere, Category = "General Attributes")
		int iDamageToDeal;
	UPROPERTY(BlueprintReadOnly, Category = "General Attributes")
		float fLifetime;
	UPROPERTY(EditAnywhere, Category = "General Attributes")
		bool bCanDamagePlayer;
	UPROPERTY(EditAnywhere, Category = "General Attributes")
		bool bCanDamageBoss;
	UPROPERTY(EditAnywhere, Category = "General Attributes")
		TSubclassOf<ADecalActor> HitDecal;
	UPROPERTY(EditAnywhere, Category = "General Attributes")
		FName SourceName;

	UPROPERTY(EditAnywhere, Category = "General Attributes")
		USoundCue* SoundOnKill;
	UPROPERTY(EditAnywhere, Category = "General Attributes")
		UParticleSystem* ParticleOnKill;

	UPROPERTY(EditAnywhere, Category = "Movement Attributes")
		float fTravelSpeed;
	UPROPERTY(EditAnywhere, Category = "Movement Attributes")
		float fGravityScale;

protected:	

	// Methods

	void SpawnDecal(FHitResult Hit);

	// Variables
	
	TQueue<ANewProjectileBase*>* ReturnQueue;
	TArray<TSubclassOf<AActor>> ClassesToIgnore;
	int iBaseDamage;
	FTimerHandle LifetimeHandle;

};