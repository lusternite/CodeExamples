// Fill out your copyright notice in the Description page of Project Settings.

#include "Introspect.h"
#include "CobaltHomingBullet.h"

ACobaltHomingBullet::ACobaltHomingBullet()
	: Super()
{

}

void ACobaltHomingBullet::SetHomingTarget(UPrimitiveComponent* target)
{
	ProjectileMovementComponent->HomingTargetComponent = target;
	GetWorldTimerManager().SetTimer(HomingDelayHandle, this, &ACobaltHomingBullet::StartHoming, HomingDelay, false);
}

void ACobaltHomingBullet::OnHitActor(AActor * actor, const FHitResult & SweepResult)
{
	EHitType HitType = Cast<ICanTakeDamage>(actor)->Execute_ObjectTakeDamage(actor, iDamageToDeal, 1.0f, SweepResult, SourceName);

	// Clear the timer and destroy
	GetWorldTimerManager().ClearTimer(LifetimeHandle);
	OnLifetimeExpired();
}

void ACobaltHomingBullet::OnHitComponent(UPrimitiveComponent * component, const FHitResult & SweepResult)
{
	EHitType HitType = Cast<ICanTakeDamage>(component)->Execute_ObjectTakeDamage(component, iDamageToDeal, 1.0f, SweepResult, SourceName);

	// Clear the timer and destroy
	GetWorldTimerManager().ClearTimer(LifetimeHandle);
	OnLifetimeExpired();
}

void ACobaltHomingBullet::StartHoming()
{
	ProjectileMovementComponent->bIsHomingProjectile = true;
	ProjectileMovementComponent->HomingAccelerationMagnitude = HomingMagnitude;
	ProjectileMovementComponent->MaxSpeed = SpeedAfterHoming;
}