// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerController.h"
#include "IntrospectPlayerController.generated.h"

/**
 * 
 */
UCLASS()
class INTROSPECT_API AIntrospectPlayerController : public APlayerController
{
	GENERATED_BODY()

public:

	virtual void BeginPlay() override;

protected:


};
