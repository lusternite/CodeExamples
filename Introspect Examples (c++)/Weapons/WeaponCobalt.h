// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameEntities/Weapons/WeaponBase.h"
#include "WeaponCobalt.generated.h"

/**
 * 
 */
UCLASS()
class INTROSPECT_API AWeaponCobalt : public AWeaponBase
{
	GENERATED_BODY()
	
public:

	// Methods

	AWeaponCobalt();

	virtual void SetSecondaryIsFiring(bool b) override;
	virtual void SetPrimaryIsFiring(bool b) override;
	virtual void Upgrade(EUpgradeType UpgradeType) override;


	// Variables



protected:

	// Methods

	virtual void Tick(float DeltaTime) override;
	virtual void BeginPlay() override;

	virtual void FirePrimary() override;
	virtual void FireSecondary() override;
	virtual void OnOverheatBegin() override;
	virtual void Unequip() override;
	virtual void SubtractOverheat(float Value) override;

	virtual void OnHitActor(AActor* actor, FHitScanFireParamaters params, FHitResult hit) override;
	virtual void OnHitComponent(UPrimitiveComponent* component, FHitScanFireParamaters params, FHitResult hit) override;

	void StopLazer();
	void StartLazer();

	// Variables

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		FHitScanFireParamaters PrimaryParams;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		FHitScanFireParamaters SecondaryParams;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		UAnimSequence* PrimaryAnim;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		UMaterialInterface* LazerMat;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		float TimeBetweenLazerTicks;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		UParticleSystem* PrimaryTrail;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		UAudioComponent* ChargingSound;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		UAudioComponent* LazerSound;

	UPROPERTY(BlueprintReadOnly, Category = "Runtime Variables")
		bool bLazerFiring;
	UPROPERTY(BlueprintReadOnly, Category = "Runtime Variables")
		FVector HitPoint;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Components")
		UStaticMeshComponent* Lazer;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Components")
		UParticleSystemComponent* MuzzleFlash;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Components")
		UParticleSystemComponent* LazerCharge;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Upgrade Attributes")
		FHitScanFireParamaters PiercingLazerParam;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Upgrade Attributes")
		UMaterialInterface* PiercingLazerMat;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Upgrade Attributes")
		FBulletFireParamaters HomingBulletParams;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Upgrade Attributes")
		int MaxTargetedComponents;

	float TimeToFire;

private:

	// Methods

	void PerformPiercingHitScan();
	void FireNormalLazer();
	void SpawnHomingBullets(FHitResult Hit);

	void AddTargetToArray(UPrimitiveComponent* Target);
	void ShootTargets();
	void ClearArray();

	// Variables

	float OverheatOnBeginCharge;
	float CurrentTimeBetweenLazerTick;
	TArray<UPrimitiveComponent*> OnOverheatTargetedComponents;

	EHitType Highest;
	bool isPiercing;
};
