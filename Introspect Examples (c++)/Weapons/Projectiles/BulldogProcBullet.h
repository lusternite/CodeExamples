// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameEntities/Projectiles/NewBullets/NewProjectileBase.h"
#include "BulldogProcBullet.generated.h"

/**
 * 
 */
UCLASS()
class INTROSPECT_API ABulldogProcBullet : public ANewProjectileBase
{
	GENERATED_BODY()
	
public:

	// Functions

	ABulldogProcBullet();
	virtual void Tick(float DeltaTime) override;
	virtual void BeginPlay() override;
	
	// Variables



protected:

	// Functions



	// Variables

	UPROPERTY(EditAnywhere, Category = "Components")
		UParticleSystemComponent * TravelParticle;

	UPROPERTY(EditAnywhere, Category = "Attributes")
		float RadiusMax;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float RadiusMin;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float DegreesPerSecondMax;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float DegreesPerSecondMin;
	UPROPERTY(EditAnywhere, Category = "Attributes")
		float SpeedVariance;

private:

	// Functions

	void HandleAngle(float DeltaTime);
	FVector MakeVector();
	void AddOffset(FVector UnadjustedVector);

	// Variables

	float CurrentDegree;
	float DegreesPerSecond;
	float Radius;
	bool Direction;
	FVector PrevOffset;

};
