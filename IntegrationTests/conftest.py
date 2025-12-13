# -*- coding: utf-8 -*-
"""
Configuration pytest pour les tests d'integration
Pattern: Page Object Model avec Selenium
Framework: pytest + Selenium
Style: my_test mini project
"""
import pytest
import os
import sys
from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from datetime import datetime

# Ajouter le repertoire parent au PYTHONPATH pour permettre les imports
CURRENT_DIR = os.path.dirname(os.path.abspath(__file__))
if CURRENT_DIR not in sys.path:
    sys.path.insert(0, CURRENT_DIR)

# Importer ReadConfig après avoir ajouté le path
from utilities.readProperties import ReadConfig

# Créer les répertoires nécessaires
os.makedirs('./Screenshots', exist_ok=True)
os.makedirs('./Logs', exist_ok=True)


@pytest.fixture()
def setup():
    """
    Fixture Pytest pour configurer le navigateur WebDriver.
    Cette fixture initialise une instance de WebDriver pour le navigateur Chrome.
    
    Returns:
        webdriver.Chrome: Une instance du WebDriver pour Chrome.
    
    Avantage: permet de réutiliser le code de configuration dans plusieurs tests,
    de séparer le code de configuration du code de test, et de gérer automatiquement
    le cycle de vie des objets nécessaires pour les tests.
    """
    # Configuration Chrome
    chrome_options = Options()
    chrome_options.add_argument('--no-sandbox')
    chrome_options.add_argument('--disable-dev-shm-usage')
    chrome_options.add_argument('--window-size=1920,1080')
    chrome_options.add_argument('--disable-gpu')
    
    # Initialiser le driver
    driver = webdriver.Chrome(options=chrome_options)
    driver.implicitly_wait(ReadConfig.getImplicitWait())
    
    return driver


@pytest.fixture(scope='session')
def base_url():
    """URL de base pour le frontend"""
    return ReadConfig.getBaseURL()


@pytest.fixture(scope='session')
def api_url():
    """URL de base pour l'API backend"""
    return ReadConfig.getApiURL()


@pytest.hookimpl(tryfirst=True, hookwrapper=True)
def pytest_runtest_makereport(item, call):
    """Hook pour capturer le statut du test"""
    outcome = yield
    rep = outcome.get_result()
    setattr(item, f'rep_{rep.when}', rep)


# Fixtures pour les tests API (si nécessaire)
@pytest.fixture(scope='function')
def auth_token(api_url):
    """
    Fixture pour obtenir un token d'authentification valide.
    Utilise pour les tests API necessitant une authentification.
    """
    import requests
    
    login_data = {
        'username': ReadConfig.getAdminUsername(),
        'password': ReadConfig.getAdminPassword()
    }
    
    try:
        response = requests.post(f'{api_url}/api/auth/login', json=login_data)
        if response.status_code == 200:
            return response.json().get('token')
    except Exception as e:
        print(f'Erreur lors de l\'obtention du token: {e}')
    
    return None


@pytest.fixture(scope='function')
def api_client(api_url, auth_token):
    """
    Fixture pour un client API avec authentification.
    Retourne une session requests avec headers configures.
    """
    import requests
    
    session = requests.Session()
    if auth_token:
        session.headers.update({
            'Authorization': f'Bearer {auth_token}',
            'Content-Type': 'application/json'
        })
    
    session.base_url = api_url
    return session
