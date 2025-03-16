// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "BulldogProcBullet.h"


ABulldogProcBullet::ABulldogProcBullet()
	: Super()
{
	TravelParticle = CreateDefaultSubobject<UParticleSystemComponent>("Travel Particle");
}

void ABulldogProcBullet::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	HandleAngle(DeltaTime);
	FVector v = MakeVector();
	AddOffset(v);
}

void ABulldogProcBullet::BeginPlay()
{
	Super::BeginPlay();

	Radius = FMath::RandRange(RadiusMin, RadiusMax);
	DegreesPerSecond = FMath::RandRange(DegreesPerSecondMin, DegreesPerSecondMax);
	CurrentDegree = FMath::RandRange(0.0f, 359.0f);
	Direction = FMath::RandBool();

	float AdjustedSpeed = FMath::RandRange(fTravelSpeed - SpeedVariance, fTravelSpeed + SpeedVariance);
	ProjectileMovementComponent->MaxSpeed = AdjustedSpeed;
}

void ABulldogProcBullet::HandleAngle(float DeltaTime)
{
	float NewDegree = 0.0f;

	if (Direction)
	{
		NewDegree = CurrentDegree + (DeltaTime * DegreesPerSecond);
	}
	else
	{
		NewDegree = CurrentDegree - (DeltaTime * DegreesPerSecond);
	}

	if (NewDegree >= 360.0f)
	{
		CurrentDegree = (360.0f - NewDegree);
	}
	else if (NewDegree < 0.0f)
	{
		CurrentDegree = (360.0f + NewDegree);
	}
	else
	{
		CurrentDegree = NewDegree;
	}
}

FVector ABulldogProcBullet::MakeVector()
{
	float y = FMath::Sin(CurrentDegree) * Radius;
	float z = FMath::Cos(CurrentDegree) * Radius;
	FVector v = { 0.0f, y, z };

	return v;
}

void ABulldogProcBullet::AddOffset(FVector UnadjustedVector)
{
	FVector v = UnadjustedVector - PrevOffset;
	StaticMeshComponent->AddLocalOffset(v);
	PrevOffset = UnadjustedVector;
}