#pragma once

#include "Bullet.h"


class CBlast : public CBullet
{
public:
	//Member function
	CBlast();
	~CBlast();

	virtual bool Initialise(float _fPosX, float _fPosY, float _fVelocityX, float _fVelocityY);
};

