# -*- coding: utf-8 -*-
"""
Configuration pytest pour les tests d'integration
Pattern: Page Object Model avec Selenium
Framework: pytest + Selenium
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

# Configuration de base
BASE_URL = os.getenv('BASE_URL', 'http://localhost:5000')
API_URL = os.getenv('API_URL', 'http://localhost:5001')


@pytest.fixture(scope='session')
def base_url():
    """URL de base pour le frontend"""
    return BASE_URL


@pytest.fixture(scope='session')
def api_url():
    """URL de base pour l'API backend"""
    return API_URL


@pytest.fixture(scope='function')
def driver(request):
    """
    Fixture Selenium WebDriver avec gestion automatique
    - Configuration Chrome
    - Screenshots en cas d'echec
    - Cleanup automatique
    """
    # Configuration Chrome
    chrome_options = Options()
    # Decommenter pour mode headless (sans interface graphique)
    # chrome_options.add_argument('--headless')
    chrome_options.add_argument('--no-sandbox')
    chrome_options.add_argument('--disable-dev-shm-usage')
    chrome_options.add_argument('--window-size=1920,1080')
    chrome_options.add_argument('--disable-gpu')
    
    # Initialiser le driver
    driver = webdriver.Chrome(options=chrome_options)
    driver.implicitly_wait(10)
    
    # Retourner le driver pour le test
    yield driver
    
    # Cleanup: prendre screenshot si le test echoue
    if hasattr(request.node, 'rep_call') and request.node.rep_call.failed:
        timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
        test_name = request.node.name
        screenshot_dir = 'IntegrationTests/screenshots'
        os.makedirs(screenshot_dir, exist_ok=True)
        screenshot_path = f'{screenshot_dir}/{test_name}_{timestamp}.png'
        driver.save_screenshot(screenshot_path)
        print(f'\nScreenshot saved: {screenshot_path}')
    
    # Fermer le driver
    driver.quit()


@pytest.hookimpl(tryfirst=True, hookwrapper=True)
def pytest_runtest_makereport(item, call):
    """Hook pour capturer le statut du test"""
    outcome = yield
    rep = outcome.get_result()
    setattr(item, f'rep_{rep.when}', rep)


@pytest.fixture(scope='function')
def auth_token(api_url):
    """
    Fixture pour obtenir un token d'authentification valide
    Utilise pour les tests API necessitant une authentification
    """
    import requests
    
    login_data = {
        'username': 'admin',
        'password': 'Admin@123'
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
    Fixture pour un client API avec authentification
    Retourne une session requests avec headers configures
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
