#pragma once

#include "Invader.h"
#include <vector>
#include <random>
using namespace std;

class CFleet
{
public:
	CFleet();
	~CFleet();
	
	vector<CInvader*>& GetInvaders();
	bool IsEliminated();
	bool ShouldFrontFire();

private:


	//member variables
public:

private:
	vector<CInvader*> m_pvInvaders;

};

