import pytest 
from selenium import webdriver  
@pytest.fixture()
def setup():
    driver = webdriver.Chrome() 
    driver.current_url
    return driver 
 # Retourne l'instance de WebDriver pour utilisation dans les tests
""""
Dans Pytest, une "fixture" est une fonction spéciale utilisée pour préparer l'état nécessaire pour les tests. 
Les fixtures peuvent être utilisées pour configurer des objets ou des environnements, 
comme l'initialisation de navigateurs Web, la connexion à une base de données, ou la préparation de fichiers de test
"""
"""
    Fixture Pytest pour configurer le navigateur WebDriver. Cette fixture initialise une instance de WebDriver 
    pour le navigateur Chrome.
    La fixture est utilisée pour fournir un objet driver aux tests qui en ont besoin.
    
    Returns:
        webdriver.Chrome: Une instance du WebDriver pour Chrome.


    Avantage : permet de réutiliser le code de configuration dans plusieurs tests, 
    de séparer le code de configuration du code de test, et de gérer automatiquement 
    le cycle de vie des objets nécessaires pour les tests.
    """