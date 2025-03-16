// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameEntities/Projectiles/NewBullets/NewProjectileBase.h"
#include "CobaltHomingBullet.generated.h"

/**
 * 
 */
UCLASS()
class INTROSPECT_API ACobaltHomingBullet : public ANewProjectileBase
{
	GENERATED_BODY()

public:

	ACobaltHomingBullet();
	void SetHomingTarget(UPrimitiveComponent* target);

	virtual void OnHitActor(AActor* actor, const FHitResult &SweepResult) override;
	virtual void OnHitComponent(UPrimitiveComponent* component, const FHitResult &SweepResult) override;

protected:

	UPROPERTY(EditAnywhere, Category = "General Attributes")
		float HomingDelay;
	UPROPERTY(EditAnywhere, Category = "General Attributes")
		float SpeedAfterHoming;
	UPROPERTY(EditAnywhere, Category = "General Attributes")
		float HomingMagnitude;

private:
	
	void StartHoming();

	FTimerHandle HomingDelayHandle;

};
