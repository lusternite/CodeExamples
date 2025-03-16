// Library Includes

// Local Includes


// This Include
#include "Level.h"

// Static Variables

// Static Function Prototypes

// Implementation

// #define CHEAT_BOUNCE_ON_BACK_WALL

CLevel::CLevel()
: m_iInvadersRemaining(0)
, m_pDefender(0)
, m_iWidth(0)
, m_iHeight(0)
, m_fpsCounter(0)
{

}

CLevel::~CLevel()
{
	while (m_vpFleets.size() > 0)
	{
		while (m_vpFleets.back()->GetInvaders().size() > 0) {
			CInvader* _pInvader = m_vpFleets.back()->GetInvaders().back();

			m_vpFleets.back()->GetInvaders().pop_back();

			delete _pInvader;
		}
		CFleet* _pFleet = m_vpFleets.back();

		m_vpFleets.pop_back();

		delete _pFleet;
	}

	while (m_lpBullets.size() > 0)
	{
		CBullet* _pBullet = m_lpBullets.back();

		m_lpBullets.pop_back();

		delete _pBullet;
	}

	delete m_pDefender;
	m_pDefender = 0;

	delete m_fpsCounter;
	m_fpsCounter = 0;

	delete m_pBackground;
	m_pBackground = 0;

	if (m_iScore > m_iHighScore) {
		m_iHighScore = m_iScore;
	}
	ofstream HighScoreFile;
	HighScoreFile.open("Highscore.txt");
	HighScoreFile << ToString(m_iHighScore);
	HighScoreFile.close();
}

bool
CLevel::Initialise(int _iWidth, int _iHeight)
{
	m_iWidth = _iWidth;
	m_iHeight = _iHeight;
	m_eInvadersDirection = Right;
	m_fInvaderTimer = 0.8f;
	m_fDefenderTimer = 1.0f;

	const float fBulletVelX = 200.0f;
	const float fBulletVelY = 75.0f;
	m_fInvaderSpeed = 30;

	m_pBackground = new CSprite();
	VALIDATE(m_pBackground->Initialise(IDB_BACKGROUND, IDB_BACKGROUND));
	//Set the background position to start from {0,0}
	m_pBackground->SetX((float)m_iWidth / 2);
	m_pBackground->SetY((float)m_iHeight / 2);

	m_pTitle = new CSprite();
	VALIDATE(m_pTitle->Initialise(IDB_TITLE, IDB_TITLE));
	m_pTitle->SetX((float)m_iWidth / 2);
	m_pTitle->SetY((float)m_iHeight / 2);

	m_pInstructions = new CSprite();
	VALIDATE(m_pInstructions->Initialise(IDB_INSTRUCTIONS, IDB_INSTRUCTIONS));
	m_pInstructions->SetX((float)m_iWidth / 2);
	m_pInstructions->SetY((float)m_iHeight / 2);

	m_pDefeat = new CSprite();
	VALIDATE(m_pDefeat->Initialise(IDB_DEFEAT, IDB_DEFEAT));
	m_pDefeat->SetX((float)m_iWidth / 2);
	m_pDefeat->SetY((float)m_iHeight / 2);

	m_pVictory = new CSprite();
	VALIDATE(m_pVictory->Initialise(IDB_VICTORY, IDB_VICTORY));
	m_pVictory->SetX((float)m_iWidth / 2);
	m_pVictory->SetY((float)m_iHeight / 2);

	m_eGameState = Title;
	PlaySound(L"..\\Sounds\\WarForTheWorld.wav", NULL, SND_LOOP | SND_ASYNC);

	/*m_pBullet = new CBullet();
	VALIDATE(m_pBullet->Initialise(m_iWidth / 2.0f, m_iHeight / 2.0f, fBulletVelX, fBulletVelY));*/

	m_pBunker = new CBunker();
	VALIDATE(m_pBunker->Initialise(_iWidth / 2.0f, 550.0f));

	m_pDefender = new CDefender();
	VALIDATE(m_pDefender->Initialise());

	// Set the Defender's position to be centered on the x, 
	// and a little bit up from the bottom of the window.
	m_pDefender->SetX(_iWidth / 2.0f);
	m_pDefender->SetY(_iHeight - (1.5f * m_pDefender->GetHeight()) - 30);

	m_pMothership = new CMothership();
	VALIDATE(m_pMothership->Initialise());

	m_pMothership->SetX(_iWidth / 2.0f);
	m_pMothership->SetY((m_pMothership->GetHeight() / 2));
	m_strMothershipHealth = "Mothership health: " + ToString(m_pMothership->GetHealth() * 537);

	const int kiNumInvaders = 35;
	const int kiStartX = 50;
	const int kiGap = 50;

	int iCurrentX = kiStartX;
	int iCurrentY = kiStartX;

	for (int i = 0; i < 7; ++i)
	{
		CFleet* _pFleet = new CFleet();
		m_vpFleets.push_back(_pFleet);
		vector<CInvader*> _vpInvaders = m_vpFleets[i]->GetInvaders();

		//Destroyer
		CInvader* _pDestroyer = new CDestroyer;
		VALIDATE(_pDestroyer->Initialise());

		_pDestroyer->SetX(static_cast<float>(iCurrentX));
		_pDestroyer->SetY(static_cast<float>(iCurrentY));

		iCurrentY += _pDestroyer->GetHeight() + 20;
		m_vpFleets[i]->GetInvaders().push_back(_pDestroyer);

		//Shooter
		CInvader* _pShooter = new CShooter;
		VALIDATE(_pShooter->Initialise());

		_pShooter->SetX(static_cast<float>(iCurrentX));
		_pShooter->SetY(static_cast<float>(iCurrentY));

		iCurrentY += _pShooter->GetHeight() + 20;
		m_vpFleets[i]->GetInvaders().push_back(_pShooter);

		//Splitter
		CInvader* _pSplitter = new CSplitter;
		VALIDATE(_pSplitter->Initialise());

		_pSplitter->SetX(static_cast<float>(iCurrentX));
		_pSplitter->SetY(static_cast<float>(iCurrentY));

		iCurrentY += _pSplitter->GetHeight() + 20;
		m_vpFleets[i]->GetInvaders().push_back(_pSplitter);

		//Squid
		CInvader* _pSquid = new CSquid;
		VALIDATE(_pSquid->Initialise());

		_pSquid->SetX(static_cast<float>(iCurrentX));
		_pSquid->SetY(static_cast<float>(iCurrentY));

		iCurrentY += _pSquid->GetHeight() + 20;
		m_vpFleets[i]->GetInvaders().push_back(_pSquid);

		//Invader
		CInvader* _pInvader = new CInvader();
		VALIDATE(_pInvader->Initialise());

		_pInvader->SetX(static_cast<float>(iCurrentX));
		_pInvader->SetY(static_cast<float>(iCurrentY));

		iCurrentY += _pInvader->GetHeight() + 20;
		m_vpFleets[i]->GetInvaders().push_back(_pInvader);

		iCurrentX += static_cast<int>(m_vpFleets[i]->GetInvaders()[0]->GetWidth()) + kiGap;
		iCurrentY = kiStartX;
	}
	UpdateScoreText();
	m_iScore = 0;
	m_iLives = 3;
	m_strLives = "Lives: " + ToString(m_iLives);
	m_pUFO = NULL;

	m_dShootCooldown = 0;
	m_dInvaderCooldown = 0;
	__int64 _TimerFrequency;
	QueryPerformanceFrequency((LARGE_INTEGER*)&_TimerFrequency);
	m_dSecondsPerFrame = 1.0 / static_cast<double>(_TimerFrequency);
	SetInvadersRemaining(kiNumInvaders);
	m_fpsCounter = new CFPSCounter();
	VALIDATE(m_fpsCounter->Initialise());

	ifstream HighScoreFile;
	HighScoreFile.open("Highscore.txt");
	string _strHighScore;
	getline(HighScoreFile, _strHighScore);
	if (!_strHighScore.empty()) {
		m_iHighScore = stoi(_strHighScore);
	}
	HighScoreFile.close();

	srand(static_cast<unsigned int>(time(NULL)));
	return (true);
}

bool CLevel::ResetGame() {
	m_eInvadersDirection = Right;
	m_fInvaderTimer = 0.8f;

	m_fInvaderSpeed = 30;

	delete m_pBunker;
	m_pBunker = new CBunker();
	VALIDATE(m_pBunker->Initialise(m_iWidth / 2.0f, 550.0f));

	m_pDefender->SetX(m_iWidth / 2.0f);
	m_pDefender->SetY(m_iHeight - (1.5f * m_pDefender->GetHeight()) - 30);

	delete m_pMothership;
	m_pMothership = new CMothership();
	VALIDATE(m_pMothership->Initialise());

	m_pMothership->SetX(m_iWidth / 2.0f);
	m_pMothership->SetY((m_pMothership->GetHeight() / 2));
	m_strMothershipHealth = "Mothership health: " + ToString(m_pMothership->GetHealth() * 537);

	const int kiNumInvaders = 35;
	const int kiStartX = 50;
	const int kiGap = 50;

	int iCurrentX = kiStartX;
	int iCurrentY = kiStartX;

	for (int i = 0; i < m_vpFleets.size(); ++i)
	{
		vector<CInvader*> _vpInvaders = m_vpFleets[i]->GetInvaders();
		for (int j = 0; j < _vpInvaders.size(); j++) {
			_vpInvaders[j]->SetX(iCurrentX);
			_vpInvaders[j]->SetY(iCurrentY);
			_vpInvaders[j]->SetHit(false);

			iCurrentY += _vpInvaders[j]->GetHeight() + 20;
		}

		iCurrentX += static_cast<int>(m_vpFleets[i]->GetInvaders()[0]->GetWidth()) + kiGap;
		iCurrentY = kiStartX;
	}

	while (m_lpBullets.size() > 0)
	{
		CBullet* _pBullet = m_lpBullets.back();

		m_lpBullets.pop_back();

		delete _pBullet;
	}

	m_iScore = 0;
	UpdateScoreText();
	m_iLives = 3;
	m_strLives = "Lives: " + ToString(m_iLives);
	if (m_pUFO != NULL) {
		delete m_pUFO;
		m_pUFO = NULL;
	}

	m_dShootCooldown = 0;
	m_dInvaderCooldown = 0;

	SetInvadersRemaining(kiNumInvaders);

	srand(static_cast<unsigned int>(time(NULL)));
	return (true);
}

void
CLevel::Draw()
{
	switch (m_eGameState) {
	case Title:
	{
		m_pTitle->Draw();
		break;
	}
	case Instructions:
	{
		m_pInstructions->Draw();
		break;
	}
	case Game:
	{
		m_pBackground->Draw();
		m_pBunker->Draw();
		if (m_pUFO != NULL) {
			m_pUFO->Draw();
		}
		for (unsigned int i = 0; i < m_vpFleets.size(); ++i)
		{
			for (unsigned int j = 0; j < m_vpFleets[i]->GetInvaders().size(); ++j) {
				m_vpFleets[i]->GetInvaders()[j]->Draw();
			}
		}

		m_pDefender->Draw();

		for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); ++it)
		{
			(*it)->Draw();
		}

		DrawScore();
		DrawLives();
		DrawFPS();
		break;
	}
	case Mothership:
	{
					   m_pBackground->Draw();
					   m_pBunker->Draw();
					   m_pDefender->Draw();
					   m_pMothership->Draw();

					   for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); ++it)
					   {
						   (*it)->Draw();
					   }

					   UpdateScoreText();
					   DrawScore();
					   DrawLives();
					   DrawMothershipHealth();
					   DrawFPS();
					   break;
	}
	case Defeat:
	{
				   m_pDefeat->Draw();
				   DrawReward();
				   break;
	}
	case Victory:
	{
					m_pVictory->Draw();
					break;
	}
	default:
		break;
	}
	
}

void
CLevel::Process(float _fDeltaTick)
{
	switch (m_eGameState) {
	case Title:
	{
		__int64 currTime;
		QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
		double _dCurrentTime = static_cast<double>(currTime);
		double _dDeltaTime = (_dCurrentTime - m_dStateChangeCooldown) * m_dSecondsPerFrame;
		if (GetAsyncKeyState(VK_SPACE) && 0x8000 && _dDeltaTime > 1) {
			SetGameState(Instructions);
		}
		break;
	}
	case Instructions:
	{
		__int64 currTime;
		QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
		double _dCurrentTime = static_cast<double>(currTime);
		double _dDeltaTime = (_dCurrentTime - m_dStateChangeCooldown) * m_dSecondsPerFrame;
		if (GetAsyncKeyState(VK_SPACE) && 0x8000 && _dDeltaTime > 1) {
			PlaySound(L"..\\Sounds\\FightTheFleet.wav", NULL, SND_LOOP | SND_ASYNC);
			SetGameState(Game);
		}
		break;
	}
	case Game:
	{
		m_pBackground->Process(_fDeltaTick);

		for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); ++it) {
			(*it)->Process(_fDeltaTick);
		}

		m_pDefender->Process(_fDeltaTick);

		ProcessInvaderMovement();

		ProcessBulletSpawn();
		//ProcessBulletWallCollision();
		////ProcessDefenderWallCollison();
		ProcessBulletDefenderCollision();
		ProcessBulletInvaderCollision();
		ProcessBulletBunkerCollision();
		ProcessLazor(_fDeltaTick);
		ProcessUFO(_fDeltaTick);

		//ProcessCheckForWin();
		ProcessCheckForLose();
		ProcessBulletBounds();

		for (unsigned int i = 0; i < m_vpFleets.size(); ++i)
		{
			for (unsigned int j = 0; j < m_vpFleets[i]->GetInvaders().size(); ++j) {
				m_vpFleets[i]->GetInvaders()[j]->Process(_fDeltaTick);
			}
		}

		ProcessBulletDeletion();

		m_fpsCounter->CountFramesPerSecond(_fDeltaTick);
		break;
	}
	case Mothership:
	{
					   m_pBackground->Process(_fDeltaTick);
					   for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); ++it) {
						   (*it)->Process(_fDeltaTick);
					   }
					   m_pDefender->Process(_fDeltaTick);
					   ProcessMothershipMovement(_fDeltaTick);

					   ProcessBulletSpawn();
					   ProcessBulletDefenderCollision();
					   ProcessBulletMothershipCollision();
					   ProcessBulletBunkerCollision();
					   ProcessLazor(_fDeltaTick);
					   //ProcessCheckForWin();
					   ProcessCheckForLose();
					   ProcessBulletBounds();
					   m_pMothership->Process(_fDeltaTick);

					   ProcessBulletDeletion();

					   m_fpsCounter->CountFramesPerSecond(_fDeltaTick);
					   break;
	}
	case Defeat:
	{
				   __int64 currTime;
				   QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
				   double _dCurrentTime = static_cast<double>(currTime);
				   double _dDeltaTime = (_dCurrentTime - m_dStateChangeCooldown) * m_dSecondsPerFrame;
				   if (GetAsyncKeyState(VK_SPACE) && 0x8000 && _dDeltaTime > 1) {
					   PlaySound(L"..\\Sounds\\FightTheFleet.wav", NULL, SND_LOOP | SND_ASYNC);
					   ResetGame();
					   SetGameState(Game);
				   }
				   else if (GetAsyncKeyState(VK_RETURN) && 0x8000 && _dDeltaTime > 1) {
					   PostQuitMessage(0);
				   }
				   break;
				   
	}
	case Victory:
	{
					__int64 currTime;
					QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
					double _dCurrentTime = static_cast<double>(currTime);
					double _dDeltaTime = (_dCurrentTime - m_dStateChangeCooldown) * m_dSecondsPerFrame;
					if (GetAsyncKeyState(VK_SPACE) && 0x8000 && _dDeltaTime > 1) {
						PlaySound(L"..\\Sounds\\WarForTheWorld.wav", NULL, SND_LOOP | SND_ASYNC);
						ResetGame();
						SetGameState(Title);
					}
					else if (GetAsyncKeyState(VK_RETURN) && 0x8000 && _dDeltaTime > 1) {
						PostQuitMessage(0);
					}
					break;
	}
	default:
		break;
	}
	
}

CDefender*
CLevel::GetDefender() const
{
	return (m_pDefender);
}

//void
//CLevel::ProcessBulletWallCollision()
//{
//	float fBulletX = m_pBullet->GetX();
//	float fBulletY = m_pBullet->GetY();
//	float fBulletW = m_pBullet->GetWidth();
//	float fBulletH = m_pBullet->GetHeight();
//
//	float fHalfBulletW = fBulletW / 2;
//	float fHalfBulletH = fBulletH / 2;
//
//	if (fBulletX < fHalfBulletW)
//	{
//		m_pBullet->SetVelocityX(m_pBullet->GetVelocityX() * -1);
//	}
//	else if (fBulletX > m_iWidth - fHalfBulletW)
//	{
//		m_pBullet->SetVelocityX(m_pBullet->GetVelocityX() * -1);
//	}
//
//	if (fBulletY < fHalfBulletH)
//	{
//		m_pBullet->SetVelocityY(m_pBullet->GetVelocityY() * -1);
//	}
//
//#ifdef CHEAT_BOUNCE_ON_BACK_WALL
//	if (fBulletY  > m_iHeight - fHalfBulletH)
//	{
//		m_pBullet->SetVelocityY(m_pBullet->GetVelocityY() * -1);
//	}
//#endif //CHEAT_BOUNCE_ON_BACK_WALL
//}

void
CLevel::ProcessBulletDefenderCollision()
{
	for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); it++) {

		float fBulletR = (*it)->GetRadius();

		float fBulletX = (*it)->GetX();
		float fBulletY = (*it)->GetY();

		float fDefenderX = m_pDefender->GetX();
		float fDefenderY = m_pDefender->GetY();

		float fDefenderH = m_pDefender->GetHeight();
		float fDefenderW = m_pDefender->GetWidth();

		if ((fBulletX + fBulletR > fDefenderX - fDefenderW / 2) &&
			(fBulletX - fBulletR < fDefenderX + fDefenderW / 2) &&
			(fBulletY + fBulletR > fDefenderY - fDefenderH / 2) &&
			(fBulletY - fBulletR < fDefenderY + fDefenderH / 2))
		{
			//Hit the front side of the Invader...
			(*it)->SetDeletion(true);
			ReduceLives();
		}
	}
}

void
CLevel::ProcessBulletInvaderCollision()
{
	for (unsigned int i = 0; i < m_vpFleets.size(); ++i)
	{
		for (unsigned int j = 0; j < m_vpFleets[i]->GetInvaders().size(); j++) {

			if (!m_vpFleets[i]->GetInvaders()[j]->IsHit())
			{
				for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); it++) {

					float fBulletR = (*it)->GetRadius();
					float fBulletVelY = (*it)->GetVelocityY();

					float fBulletX = (*it)->GetX();
					float fBulletY = (*it)->GetY();

					float fInvaderX = m_vpFleets[i]->GetInvaders()[j]->GetX();
					float fInvaderY = m_vpFleets[i]->GetInvaders()[j]->GetY();

					float fInvaderH = m_vpFleets[i]->GetInvaders()[j]->GetHeight();
					float fInvaderW = m_vpFleets[i]->GetInvaders()[j]->GetWidth();

					if ((fBulletX + fBulletR > fInvaderX - fInvaderW / 2) &&
						(fBulletX - fBulletR < fInvaderX + fInvaderW / 2) &&
						(fBulletY + fBulletR > fInvaderY - fInvaderH / 2) &&
						(fBulletY - fBulletR < fInvaderY + fInvaderH / 2) &&
						(fBulletVelY < 0))
					{
						//Hit the front side of the Invader...
						(*it)->SetDeletion(true);
						m_vpFleets[i]->GetInvaders()[j]->SetHit(true);

						m_iScore += 535;
						SetInvadersRemaining(GetInvadersRemaining() - 1);
						if (m_iInvadersRemaining == 0) {
							m_eGameState = Mothership;
							__int64 currTime;
							QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
							m_dInvaderCooldown = static_cast<double>(currTime);
							PlaySound(L"..\\Sounds\\TheMothership.wav", NULL, SND_LOOP | SND_ASYNC);
						}
					}
				}
			}
		}
	}
}

void CLevel::ProcessBulletBunkerCollision() {
	if (!m_pBunker->IsDestroyed()) {
		for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); it++) {

			float fBulletR = (*it)->GetRadius();

			float fBulletX = (*it)->GetX();
			float fBulletY = (*it)->GetY();

			float fBunkerX = m_pBunker->GetX();
			float fBunkerY = m_pBunker->GetY();

			float fBunkerH = m_pBunker->GetHeight();
			float fBunkerW = m_pBunker->GetWidth();

			if ((fBulletX + fBulletR > fBunkerX - fBunkerW / 2) &&
				(fBulletX - fBulletR < fBunkerX + fBunkerW / 2) &&
				(fBulletY + fBulletR > fBunkerY - fBunkerH / 2) &&
				(fBulletY - fBulletR < fBunkerY + fBunkerH / 2))
			{
				//Hit the front side of the Invader...
				(*it)->SetDeletion(true);
				m_pBunker->TakeDamage();
			}
		}
	}
}

void CLevel::ProcessBulletMothershipCollision() {
	for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); it++) {

		float fBulletR = (*it)->GetRadius();
		float fBulletVelY = (*it)->GetVelocityY();

		float fBulletX = (*it)->GetX();
		float fBulletY = (*it)->GetY();

		float fMothershipX = m_pMothership->GetX();
		float fMothershipY = m_pMothership->GetY();

		float fMothershipH = m_pMothership->GetHeight();
		float fMothershipW = m_pMothership->GetWidth();

		if ((fBulletX + fBulletR > fMothershipX - fMothershipW / 2) &&
			(fBulletX - fBulletR < fMothershipX + fMothershipW / 2) &&
			(fBulletY + fBulletR > fMothershipY - fMothershipH / 2) &&
			(fBulletY - fBulletR < fMothershipY + fMothershipH / 2) &&
			(fBulletVelY < 0))
		{
			(*it)->SetDeletion(true);
			m_pMothership->TakeDamage();
			m_strMothershipHealth = "Mothership health: " + ToString(m_pMothership->GetHealth() * 537);
			m_iScore += 915;
		}
	}
}

void CLevel::ProcessInvaderMovement() {
	__int64 currTime;
	QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
	double _dCurrentTime = static_cast<double>(currTime);
	double _dDeltaCooldown = (_dCurrentTime - m_dInvaderCooldown) * m_dSecondsPerFrame;
	if (_dDeltaCooldown > m_fInvaderTimer) {
		if (m_eInvadersDirection == Right) {
			//Get the fleet closest to boundary that hasn't been eliminated
			int _iBoundaryFleet = m_vpFleets.size() - 1;
			for (int i = _iBoundaryFleet; i >= 0; i--) {
				if (!m_vpFleets[i]->IsEliminated()) {
					_iBoundaryFleet = i;
					break;
				}
			}
			//Check to see if invaders reached boundary
			if (m_vpFleets[_iBoundaryFleet]->GetInvaders().back()->GetX() + m_fInvaderSpeed > m_iWidth - 50) {
				//Update invader direction and y position
				m_eInvadersDirection = Left;
				for (int i = 0; i < m_vpFleets.size(); i++) {
					for (int j = 0; j < m_vpFleets[i]->GetInvaders().size(); j++) {
						m_vpFleets[i]->GetInvaders()[j]->TranslateY(m_fInvaderSpeed);
					}
				}
			}
			else {
				//Update x position
				for (int i = 0; i < m_vpFleets.size(); i++) {
					for (int j = 0; j < m_vpFleets[i]->GetInvaders().size(); j++) {
						m_vpFleets[i]->GetInvaders()[j]->TranslateX(m_fInvaderSpeed);
					}
				}
			}
		}
		else {
			//Get the fleet closest to boundary that hasn't been eliminated
			int _iBoundaryFleet = 0;
			for (int i = _iBoundaryFleet; i < m_vpFleets.size(); i++) {
				if (!m_vpFleets[i]->IsEliminated()) {
					_iBoundaryFleet = i;
					break;
				}
			}
			//Check to see if invaders reached boundary
			if (m_vpFleets[_iBoundaryFleet]->GetInvaders().back()->GetX() - m_fInvaderSpeed < 50) {
				//Update invader direction and y position
				m_eInvadersDirection = Right;
				for (int i = 0; i < m_vpFleets.size(); i++) {
					for (int j = 0; j < m_vpFleets[i]->GetInvaders().size(); j++) {
						m_vpFleets[i]->GetInvaders()[j]->TranslateY(m_fInvaderSpeed);
					}
				}
			}
			else {
				//Update x position
				for (int i = 0; i < m_vpFleets.size(); i++) {
					for (int j = 0; j < m_vpFleets[i]->GetInvaders().size(); j++) {
						m_vpFleets[i]->GetInvaders()[j]->TranslateX(-m_fInvaderSpeed);
					}
				}
			}
		}
		m_dInvaderCooldown = _dCurrentTime;
	}
}

void CLevel::ProcessMothershipMovement(float _fDeltaTime) {
	if (m_eInvadersDirection == Right) {
		//Check to see if mothership reached boundary
		if (m_pMothership->GetX() + m_fInvaderSpeed * _fDeltaTime > m_iWidth - 50) {
			//Update mothership direction and y position
			m_eInvadersDirection = Left;
			m_fInvaderSpeed = rand() % 150 + 50;
			m_pMothership->TranslateX(-m_fInvaderSpeed * _fDeltaTime * 5);
		}
		else {
			//Update x position
			m_pMothership->TranslateX(m_fInvaderSpeed * _fDeltaTime * 5);
		}
	}
	else {
		//Check to see if mothership reached boundary
		if (m_pMothership->GetX() - m_fInvaderSpeed * _fDeltaTime < 50) {
			//Update mothership direction and y position
			m_eInvadersDirection = Right;
			m_fInvaderSpeed = rand() % 150 + 50;
			m_pMothership->TranslateX(m_fInvaderSpeed * _fDeltaTime * 5);
		}
		else {
			//Update x position
			m_pMothership->TranslateX(-m_fInvaderSpeed * _fDeltaTime * 5);
		}
	}
	__int64 currTime;
	QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
	double _dCurrentTime = static_cast<double>(currTime);
	double _dDeltaCooldown = (_dCurrentTime - m_dInvaderCooldown) * m_dSecondsPerFrame;
	if (_dDeltaCooldown > 30) {
		m_pMothership->TranslateY(50);
		m_dInvaderCooldown = _dCurrentTime;
	}
}

void CLevel::ProcessLazor(float _fDeltaTick) {
	for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); it++) {
		if ((*it)->IsLazor()) {
			if ((*it)->CanSpawnTail()) {
				CBullet* _pTail = (*it)->SpawnTail(_fDeltaTick);
				m_lpBullets.push_back(_pTail);
			}
		}
	}
}

void
CLevel::ProcessCheckForWin()
{
	for (unsigned int i = 0; i < m_vpFleets.size(); ++i)
	{
		for (unsigned int j = 0; j < m_vpFleets[i]->GetInvaders().size(); ++j) {
			if (!m_vpFleets[i]->GetInvaders()[j]->IsHit()) {
				return;
			}
		}
	}

	CGame::GetInstance().GameOverWon();
}

void
CLevel::ProcessBulletBounds()
{
	for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); it++) {
		if ((*it)->IsBounce()) {
			//if ((*it)->GetX() < 0)
			//{
			//	m_pBullet->SetX(0);
			//}
			//else if (m_pBullet->GetX() > m_iWidth)
			//{
			//	m_pBullet->SetX(static_cast<float>(m_iWidth));
			//}

			//if (m_pBullet->GetY() < 0)
			//{
			//	m_pBullet->SetY(0.0f);
			//}
			//else if (m_pBullet->GetY() > m_iHeight)
			//{
			//	CGame::GetInstance().GameOverLost();
			//	//m_pBullet->SetY(static_cast<float>(m_iHeight));
			//}
		}
		else {
			if ((*it)->GetX() < 0)
			{
				(*it)->SetDeletion(true);
			}
			else if ((*it)->GetX() > m_iWidth)
			{
				(*it)->SetDeletion(true);
			}

			if ((*it)->GetY() < 0)
			{
				(*it)->SetDeletion(true);
			}
			else if ((*it)->GetY() > m_iHeight)
			{
				(*it)->SetDeletion(true);
				//m_pBullet->SetY(static_cast<float>(m_iHeight));
			}
		}
	}
}

void CLevel::ProcessBulletSpawn() {
	//Player bullet spawn
	if (GetAsyncKeyState(VK_SPACE) && 0x8000) {
		__int64 currTime;
		QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
		double _dCurrentTime = static_cast<double>(currTime);
		double _dDeltaCooldown = (_dCurrentTime - m_dShootCooldown) * m_dSecondsPerFrame;
		if (_dDeltaCooldown >= m_fDefenderTimer) {
			CBullet* _pBullet = new CBullet();
			_pBullet->Initialise(m_pDefender->GetX(), m_pDefender->GetY() - 20, 0, -500);
			m_lpBullets.push_back(_pBullet);
			m_dShootCooldown = _dCurrentTime;
			//Fire blast shot
			if (m_pDefender->HasBlast()) {
				CBullet* _pBlast = new CBlast ();
				_pBlast->Initialise(m_pDefender->GetX(), m_pDefender->GetY() - 40, rand() % 300 - 150, -600);
				m_lpBullets.push_back(_pBlast);
			}
			//Fire weave shot
			if (m_pDefender->HasWeave()) {
				CBullet* _pWeave = new CWeave();
				_pWeave->Initialise(m_pDefender->GetX(), m_pDefender->GetY() - 40, 0, -600);
				m_lpBullets.push_back(_pWeave);
			}
			//Fire thunder shot
			if (m_pDefender->HasThunder()) {
				CBullet* _pThunder = new CThunder();
				_pThunder->Initialise(m_pDefender->GetX(), m_pDefender->GetY() - 40, rand() % 300 - 150, -600);
				m_lpBullets.push_back(_pThunder);
			}
		}
	}
	//Invader bullet spawn
	if (m_eGameState == Game) {

		for (int i = 0; i < m_vpFleets.size(); i++) {
			if (m_vpFleets[i]->ShouldFrontFire()) {
				int _iFrontShip = m_vpFleets[i]->GetInvaders().size() - 1;
				for (int j = m_vpFleets[i]->GetInvaders().size() - 1; j >= 0; j--) {
					if (!m_vpFleets[i]->GetInvaders()[j]->IsHit()) {
						_iFrontShip = j;
						break;
					}
				}
				if (_iFrontShip == 0) {
					for (int k = 0; k < 3; k++) {
						CBullet* _pBullet = m_vpFleets[i]->GetInvaders()[_iFrontShip]->FireBullet();
						m_lpBullets.push_back(_pBullet);
					}
				}
				else {
					CBullet* _pBullet = m_vpFleets[i]->GetInvaders()[_iFrontShip]->FireBullet();
					m_lpBullets.push_back(_pBullet);
				}
			}
		}
	}
	else {
		if (rand() % 50 == 0) {
			CBullet* _pBullet = m_pMothership->FireBullet();
			m_lpBullets.push_back(_pBullet);
		}
	}
}

bool BulletDeletionCheck(CBullet* _pBullet) {
	if (_pBullet == NULL) {
		return true;
	}
	else {
		return false;
	}
}

void CLevel::ProcessBulletDeletion() {
	for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); it++) {
		if ((*it)->IsDeletion()) {
			delete (*it);
			(*it) = NULL;
		}
	}
	m_lpBullets.remove_if(BulletDeletionCheck);
}

void CLevel::ProcessUFO(float _fDeltaTick) {
	if (m_pUFO != NULL) {
		//Check if UFO has been hit by a bullet
		for (list<CBullet*>::iterator it = m_lpBullets.begin(); it != m_lpBullets.end(); it++) {

			float fBulletR = (*it)->GetRadius();

			float fBulletX = (*it)->GetX();
			float fBulletY = (*it)->GetY();

			float fUFOX = m_pUFO->GetX();
			float fUFOY = m_pUFO->GetY();

			float fUFOH = m_pUFO->GetHeight();
			float fUFOW = m_pUFO->GetWidth();

			if ((fBulletX + fBulletR > fUFOX - fUFOW / 2) &&
				(fBulletX - fBulletR < fUFOX + fUFOW / 2) &&
				(fBulletY + fBulletR > fUFOY - fUFOH / 2) &&
				(fBulletY - fBulletR < fUFOY + fUFOH / 2))
			{
				//Hit the front side of the Invader...
				(*it)->SetDeletion(true);
				m_iScore += 1075;
				delete m_pUFO;
				m_pUFO = NULL;
				return;
			}
		}
		m_pUFO->Process(_fDeltaTick);
		if (m_pUFO->GetX() < 0 || m_pUFO->GetX() > m_iWidth) {
			delete m_pUFO;
			m_pUFO = NULL;
		}
	}
	else {
		int _iRandom = rand() % 3000;
		if (_iRandom == 0) {
			m_pUFO = new CUFO;
			m_pUFO->Initialise(200.0f);
			m_pUFO->SetX(m_pUFO->GetWidth());
			m_pUFO->SetY(m_pUFO->GetHeight());
		}
		else if (_iRandom == 1) {
			m_pUFO = new CUFO;
			m_pUFO->Initialise(-200.0f);
			m_pUFO->SetX(m_iWidth - m_pUFO->GetWidth());
			m_pUFO->SetY(m_pUFO->GetHeight());
		}
	}
}

int
CLevel::GetInvadersRemaining() const
{
	return (m_iInvadersRemaining);
}

void
CLevel::SetInvadersRemaining(int _i)
{
	m_iInvadersRemaining = _i;
	m_fInvaderTimer -= 0.02;
	UpdateScoreText();
}

void
CLevel::DrawScore()
{
	HDC hdc = CGame::GetInstance().GetBackBuffer()->GetBFDC();

	const int kiX = 20;
	const int kiY = 20;
	SetTextColor(hdc, RGB(255, 255, 255));
	SetBkMode(hdc, TRANSPARENT);

	TextOutA(hdc, kiX, kiY, m_strScore.c_str(), static_cast<int>(m_strScore.size()));
}



void
CLevel::UpdateScoreText()
{
	m_strScore = "Score: ";

	m_strScore += ToString(m_iScore) + " Bullets: " + ToString(m_lpBullets.size());
}

void CLevel::ReduceLives() {
	m_iLives -= 1;
	if (m_iLives == 0) {
		CGame::GetInstance().GameOverLost();
	}
	m_strLives = "Lives: " + ToString(m_iLives);
}

void CLevel::DrawLives() {
	HDC hdc = CGame::GetInstance().GetBackBuffer()->GetBFDC();

	const int kiX = 900;
	const int kiY = 20;
	SetTextColor(hdc, RGB(255, 255, 255));
	SetBkMode(hdc, TRANSPARENT);

	TextOutA(hdc, kiX, kiY, m_strLives.c_str(), static_cast<int>(m_strLives.size()));

}

void CLevel::DrawMothershipHealth() {
	HDC hdc = CGame::GetInstance().GetBackBuffer()->GetBFDC();

	const int kiX = 450;
	const int kiY = 275;
	SetTextColor(hdc, RGB(255, 0, 0));
	SetBkMode(hdc, TRANSPARENT);

	TextOutA(hdc, kiX, kiY, m_strMothershipHealth.c_str(), static_cast<int>(m_strMothershipHealth.size()));
}

void
CLevel::DrawFPS()
{
	HDC hdc = CGame::GetInstance().GetBackBuffer()->GetBFDC();

	m_fpsCounter->DrawFPSText(hdc, m_iWidth, m_iHeight);

}

void CLevel::SetGameState(GameState _eGameState) {
	__int64 currTime;
	QueryPerformanceCounter((LARGE_INTEGER*)&currTime);
	m_dStateChangeCooldown = static_cast<double>(currTime);
	m_eGameState = _eGameState;
}

void CLevel::ProcessCheckForLose() {
	int _iInvaderHeight = 0;
	for (int i = 0; i < m_vpFleets.size(); i++) {
		for (int j = 0; j < m_vpFleets[i]->GetInvaders().size(); j++) {
			if (!m_vpFleets[i]->GetInvaders()[j]->IsHit() && j > _iInvaderHeight) {
				_iInvaderHeight = j;
			}
		}
	}
	if (m_vpFleets[0]->GetInvaders()[_iInvaderHeight]->GetY() >= m_pDefender->GetY() - 20) {
		CGame::GetInstance().GameOverLost();
	}
}

void CLevel::RetrieveFromDlg(HWND _hDlg) {
	wchar_t buffer[256];
	wchar_t* end;

	HWND EditBox = GetDlgItem(_hDlg, IDC_EDIT1);
	GetWindowText(EditBox, buffer, 256);
	m_fInvaderTimer = wcstof(buffer, &end);

	EditBox = GetDlgItem(_hDlg, IDC_EDIT2);
	GetWindowText(EditBox, buffer, 256);
	m_fDefenderTimer = wcstof(buffer, &end);

	EditBox = GetDlgItem(_hDlg, IDC_EDIT3);
	GetWindowText(EditBox, buffer, 256);
	m_pDefender->SetMovementSpeed(wcstof(buffer, &end));
}

void CLevel::TransferToDlg(HWND _hDlg) {
	SetDlgItemTextA(_hDlg, IDC_EDIT1, ToString(m_fInvaderTimer).c_str());
	SetDlgItemTextA(_hDlg, IDC_EDIT2, ToString(m_fDefenderTimer).c_str());
	SetDlgItemTextA(_hDlg, IDC_EDIT3, ToString(m_pDefender->GetMovementSpeed()).c_str());
}

void CLevel::ProcessDefeatRewards() {
	int _iRandom = rand() % 4;
	switch (_iRandom) {
	case 0:
	{
			  m_pDefender->GainBlast();
			  m_strRewards = "You have gained Blast Shots!";
			  break;
	}
	case 1:
	{
			  m_pDefender->GainThunder();
			  m_strRewards = "You have gained Thunder Shots!";
			  break;
	}
	case 2:
	{
			  m_pDefender->GainWeave();
			  m_strRewards = "You have gained Weave Shots!";
			  break;
	}
	case 3:
	{

	}
	default:
		m_fDefenderTimer *= 0.8;
		m_strRewards = "You have gained faster attack speed!";
	}
	
}

void CLevel::DrawReward() {
	HDC hdc = CGame::GetInstance().GetBackBuffer()->GetBFDC();

	const int kiX = 450;
	const int kiY = 350;
	SetTextColor(hdc, RGB(255, 0, 0));
	SetBkMode(hdc, TRANSPARENT);

	TextOutA(hdc, kiX, kiY, m_strRewards.c_str(), static_cast<int>(m_strRewards.size()));
}