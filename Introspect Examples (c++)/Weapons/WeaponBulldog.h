// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameEntities/Weapons/WeaponBase.h"
#include "WeaponBulldog.generated.h"



UCLASS()
class INTROSPECT_API AWeaponBulldog : public AWeaponBase
{
	GENERATED_BODY()

public:

	AWeaponBulldog();

	UFUNCTION(BlueprintCallable, Category = "Firing")
		virtual void SetPrimaryIsFiring(bool b) override;

	virtual void BeginPlay() override;

	UFUNCTION(BlueprintCallable, Category = "Firing")
	virtual float GetBaseFireRate();

	UFUNCTION(BlueprintCallable, Category = "Firing")
		virtual float GetPrimaryFireRate();

protected:

	virtual void FirePrimary() override;
	virtual void FireSecondary() override;
	virtual void OnOverheatBegin() override;

	
	
	UPROPERTY(EditAnywhere, Category = "Attributes")
		FBulletFireParamaters PrimaryParams;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		FBulletFireParamaters SecondaryParams;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		UAnimSequence* PrimaryAnimation;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		UAnimSequence* SecondaryAnimation;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		int iNumShotsForSecondary;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float FireRateAddPerTick;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float FireRateMaximum;

	UPROPERTY(EditAnywhere, Category = "Upgrade Attributes")
		float OverheatBonusDamageMultiplier;
	UPROPERTY(EditAnywhere, Category = "Upgrade Attributes")
		float OverheatBonusThreshhold;
	UPROPERTY(EditAnywhere, Category = "Upgrade Attributes")
		FBulletFireParamaters ProcBulletParams;
	UPROPERTY(EditAnywhere, Category = "Upgrade Attributes")
		FBulletFireParamaters GreenUpgradeBulletParams;
	UPROPERTY(EditAnywhere, Category = "Upgrade Attributes")
		int iNumShotsForSecondaryUpgraded;

private:

	UPROPERTY(VisibleAnywhere, Category = "Runtime Variables")
		float BaseFireRate;

};
