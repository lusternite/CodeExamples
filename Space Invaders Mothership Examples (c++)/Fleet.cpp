#include "Fleet.h"


CFleet::CFleet()
{
}


CFleet::~CFleet()
{
}

vector<CInvader*>& CFleet::GetInvaders() {
	return m_pvInvaders;
}

bool CFleet::IsEliminated() {
	for (int i = 0; i < m_pvInvaders.size(); i++) {
		if (!m_pvInvaders[i]->IsHit()) {
			return false;
		}
	}
	return true;
}

bool CFleet::ShouldFrontFire() {
	if (IsEliminated()) {
		return false;
	}
	if (rand() % 500 == 1) {
		return true;
	}
	return false;
}