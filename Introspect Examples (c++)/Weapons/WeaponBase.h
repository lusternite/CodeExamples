// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "GameEntities/Projectiles/NewBullets/NewProjectileBase.h"
#include "WeaponBase.generated.h"

class AIntrospectCharacter;

USTRUCT(BlueprintType)
struct FBulletFireParamaters
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere)
		float SprayAmount;
	UPROPERTY(EditAnywhere)
		TSubclassOf<class ANewProjectileBase> BulletToSpawn;
	UPROPERTY(EditAnywhere)
		bool IsSmallShot;
	UPROPERTY(EditAnywhere)
		FVector LaunchOffset;
	UPROPERTY(EditAnywhere)
		int PoolIndex;
	UPROPERTY(EditAnywhere)
		float LifeTime;
	UPROPERTY(EditAnywhere)
		USoundCue* SoundOnShoot;
	UPROPERTY(EditAnywhere)
		UParticleSystem* MuzzleParticle;
};

USTRUCT(BlueprintType)
struct FHitScanFireParamaters
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere)
		float TraceDistance;
	UPROPERTY(EditAnywhere)
		int Damage;
	UPROPERTY(EditAnywhere)
		UParticleSystem* MuzzleParticle;
	UPROPERTY(EditAnywhere)
		USoundCue* SoundOnShoot;
	UPROPERTY(EditAnywhere)
		TSubclassOf<ADecalActor> HitDecal;
	UPROPERTY(EditAnywhere)
		FName SourceName;

	bool operator==(const FHitScanFireParamaters& other) {
		return (
			TraceDistance == other.TraceDistance &&
			Damage == other.Damage &&
			MuzzleParticle == other.MuzzleParticle &&
			SoundOnShoot == other.SoundOnShoot &&
			HitDecal == other.HitDecal &&
			SourceName == other.SourceName);
	}
};

UENUM(BlueprintType)
enum class EUpgradeType : uint8
{
	ERed	UMETA(DisplayName = "Red"),
	EBlue	UMETA(DisplayName = "Blue"),
	EGreen	UMETA(DisplayName = "Green")
};

UCLASS()
class INTROSPECT_API AWeaponBase : public AActor
{
	GENERATED_BODY()
	
public:	

	// Methods

	AWeaponBase();
	UFUNCTION(BlueprintCallable, Category = "Firing")
		virtual void FirePrimary();
	UFUNCTION(BlueprintCallable, Category = "Firing")
		virtual void FireSecondary();

	UFUNCTION(BlueprintCallable, Category = "Firing")
		virtual void SetPrimaryIsFiring(bool b);
	UFUNCTION(BlueprintCallable, Category = "Firing")
		virtual void SetSecondaryIsFiring(bool b);

	UFUNCTION(BlueprintCallable, Category = "General")
		virtual void Equip();
	UFUNCTION(BlueprintCallable, Category = "General")
		virtual void Unequip();

	UFUNCTION(BlueprintCallable, Category = "General")
		virtual void Upgrade(EUpgradeType UpgradeType);
	UFUNCTION(BlueprintCallable, Category = "General")
		bool HasUpgrade(EUpgradeType UpgradeType);

	TMap<ICanTakeDamage*, UObject*> GetDamageableObjectsInRange(UPrimitiveComponent* range);
	bool IsUpgradeRandomProc();

	UFUNCTION(BlueprintImplementableEvent, Category = "PerformanceTracking")
		void TookShot();
	UFUNCTION(BlueprintImplementableEvent, Category = "PerformanceTracking")
		void HitShot(UTexture2D* HitMarker);

	UFUNCTION(BlueprintImplementableEvent, Category = "UI")
		void SpawnDamageNumberMarker(FVector ImpactLocation, int Damage, EHitType HitType);

	UFUNCTION(BlueprintImplementableEvent, BlueprintCallable, Category = "Firing")
		void ToggleOverheatVFX();

	UTexture2D* HitMarkerFromHitType(EHitType HitType);

	UFUNCTION(BlueprintCallable, Category = "Death")
	virtual void ApplyDeathPhysics();

	// Variables
	
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "RuntimeVariables")
		float fCurrentOverheat;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Runtime Variables")
		bool bIsOverheated;

	TArray<TQueue<ANewProjectileBase*>*> Queues;

protected:

	// Methods

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;
	virtual void Destroyed() override;

	virtual ANewProjectileBase* FireBullet(FBulletFireParamaters params, bool counts = true);
	virtual FHitResult PerformHitScan(FHitScanFireParamaters params);
	virtual FHitResult HitScanLineTrace(FVector start, FVector end, FHitScanFireParamaters params);
	virtual void AddOverheat(float Value);
	virtual void SubtractOverheat(float Value);
	virtual void OnOverheatBegin();
	virtual void OnOverheatEnd();
	virtual void HandleFiring(float DeltaTime);
	virtual void HandleTransform(float DeltaTime);
	virtual void HandleMovement(float DeltaTime);
	virtual void OnHitActor(AActor* actor, FHitScanFireParamaters params, FHitResult hit);
	virtual void OnHitComponent(UPrimitiveComponent* component, FHitScanFireParamaters params, FHitResult hit);
	FVector GetCenterScreenLocation();
	

	UFUNCTION(BlueprintImplementableEvent, Category = "UI Interaction")
		void ReticleBounce(bool IsSmallShot);

	// Variables

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Visual Attributes")
		UMaterialInterface* Material;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Visual Attributes")
		UTexture2D* WeaponThumbnail;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Visual Attributes")
		UTexture2D* ReticleOuter;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Visual Attributes")
		UTexture2D* ReticleInner;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Visual Attributes")
		UTexture2D* UIImage;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Visual Attributes")
		UTexture2D* HitMarkerRegular;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Visual Attributes")
		UTexture2D* HitMarkerArmour;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Visual Attributes")
		UTexture2D* HitMarkerCritical;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Sounds")
		USoundCue* HitSoundRegular;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Sounds")
		USoundCue* HitSoundArmour;
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Sounds")
		USoundCue* HitSoundCritical;

	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Runtime Variables")
		bool bIsPrimaryFiring;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Runtime Variables")
		bool bIsSecondaryFiring;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Runtime Variables")
		bool bIsPrimaryQueued;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Runtime Variables")
		bool bIsSecondaryQueued;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Runtime Variables")
		bool bIsActive;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Runtime Variables")
		float fTimeSincePrimaryFire;
	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Runtime Variables")
		float fTimeSinceSecondaryFire;

	UPROPERTY(EditAnywhere, Category = "Attributes")
		FName sBarrelSocketName;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float fPrimaryFireRate;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float fSecondaryFireRate;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float fPrimaryOverheatAmount;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float fSecondaryOverheatAmount;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float fOverheatCoolAmount;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Attributes")
		TArray<TSubclassOf<AActor>> TypesToIgnore;

	UPROPERTY(EditAnywhere, Category = "Upgrade Attributes")
		float fUpgradeProcChance;

	UPROPERTY(EditAnywhere, Category="Components")
		USkeletalMeshComponent* WeaponBody;
	UPROPERTY(EditAnywhere, Category = "Components")
		USpringArmComponent* SpringArm;

	UPROPERTY()
		TArray<EUpgradeType> Upgrades;

private:	

	// Methods

	FVector GenerateSprayVector(float SprayAmount);
	ANewProjectileBase* GetBullet(TSubclassOf<class ANewProjectileBase> type, int PoolIndex);

	float ParabolicInterp(float CurrentValue, float TargetValue, float StartingValue);
	float InterpToOppositeTarget(float Current, float Opposite);

	void SpawnDecal(FHitResult Hit, TSubclassOf<ADecalActor> HitDecal);

	// Variables

	float TargetHoverYPos;
	float TargetHoverZPos;
	float CurrentHoverYPos;
	float CurrentHoverZPos;
	float HoverSpeedY;
	float HoverSpeedZ;
	float StartingHoverYPos;
	float StartingHoverZPos;

};
