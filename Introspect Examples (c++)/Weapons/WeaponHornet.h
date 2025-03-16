// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameEntities/Weapons/WeaponBase.h"
#include "WeaponHornet.generated.h"

/**
 * 
 */
UCLASS()
class INTROSPECT_API AWeaponHornet : public AWeaponBase
{
	GENERATED_BODY()
	
public:

	// Methods

	AWeaponHornet();

	UFUNCTION(BlueprintCallable, Category = "General")
		virtual void Unequip() override;

	// Variables
	


protected:

	// Methods

	virtual void FirePrimary() override;
	virtual void FireSecondary() override;

	virtual void OnOverheatBegin() override;
	virtual void OnOverheatEnd() override;

	// Variables

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Upgrade Attributes")
		float fTimeBetweenDischarges;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Upgrade Attributes")
		int iMaxDischargeAmount;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		FBulletFireParamaters PrimaryParams;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		FBulletFireParamaters SecondaryParams;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		UAnimSequence* PrimaryAnim;
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Attributes")
		UAnimSequence* SecondaryAnim;

private:

	// Methods

	void DischargeGrenade();

	// Variables

	FTimerHandle BetweenDischargeHandle;
	int iGrenadesToDischarge;
	int iCurrentGrenadeDischarge;

};
