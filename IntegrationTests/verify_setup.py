# -*- coding: utf-8 -*-
"""
Script de verification - Tests d'integration Python
Verifie que tous les fichiers sont presents et que l'environnement est pret
"""
import os
import sys

def check_file_exists(filepath, description):
    """Verifier qu'un fichier existe"""
    if os.path.exists(filepath):
        print(f"? {description}")
        return True
    else:
        print(f"? MANQUANT: {description} ({filepath})")
        return False

def check_python_package(package_name):
    """Verifier qu'un package Python est installe"""
    try:
        __import__(package_name)
        print(f"? Package {package_name} installe")
        return True
    except ImportError:
        print(f"? Package {package_name} NON installe")
        return False

def main():
    print("="*60)
    print("VERIFICATION - Tests d'integration Python")
    print("="*60)
    print()
    
    all_ok = True
    
    # Verifier Python
    print("[1] Version Python")
    print(f"  Python {sys.version}")
    print()
    
    # Verifier les fichiers de configuration
    print("[2] Fichiers de configuration")
    all_ok &= check_file_exists("conftest.py", "conftest.py")
    all_ok &= check_file_exists("pytest.ini", "pytest.ini")
    all_ok &= check_file_exists("requirements.txt", "requirements.txt")
    all_ok &= check_file_exists(".env.example", ".env.example")
    print()
    
    # Verifier les Page Objects
    print("[3] Page Object Model")
    all_ok &= check_file_exists("pages/login_page.py", "LoginPage")
    all_ok &= check_file_exists("pages/vehicles_page.py", "VehiclesPage")
    all_ok &= check_file_exists("pages/__init__.py", "pages/__init__.py")
    print()
    
    # Verifier les tests
    print("[4] Fichiers de test")
    all_ok &= check_file_exists("tests/test_auth_api.py", "Tests API Auth")
    all_ok &= check_file_exists("tests/test_vehicles_api.py", "Tests API Vehicles")
    all_ok &= check_file_exists("tests/test_login_ui.py", "Tests UI Login")
    all_ok &= check_file_exists("tests/test_vehicles_ui.py", "Tests UI Vehicles")
    all_ok &= check_file_exists("tests/__init__.py", "tests/__init__.py")
    print()
    
    # Verifier la documentation
    print("[5] Documentation")
    all_ok &= check_file_exists("README.md", "README.md")
    all_ok &= check_file_exists("PYTHON_TESTS_SUMMARY.md", "Summary")
    all_ok &= check_file_exists("IMPLEMENTATION_COMPLETE.md", "Implementation doc")
    print()
    
    # Verifier les scripts
    print("[6] Scripts d'execution")
    all_ok &= check_file_exists("run_tests.ps1", "Script PowerShell")
    all_ok &= check_file_exists("run_tests.bat", "Script Batch")
    print()
    
    # Verifier les packages Python
    print("[7] Packages Python requis")
    all_ok &= check_python_package("pytest")
    all_ok &= check_python_package("selenium")
    all_ok &= check_python_package("requests")
    print()
    
    # Compter les tests
    print("[8] Statistiques")
    test_files = ['tests/test_auth_api.py', 'tests/test_vehicles_api.py', 
                  'tests/test_login_ui.py', 'tests/test_vehicles_ui.py']
    total_tests = 0
    for test_file in test_files:
        if os.path.exists(test_file):
            with open(test_file, 'r', encoding='utf-8') as f:
                content = f.read()
                test_count = content.count('def test_')
                total_tests += test_count
                print(f"  {test_file}: {test_count} tests")
    print(f"  TOTAL: {total_tests} tests")
    print()
    
    # Resultat final
    print("="*60)
    if all_ok:
        print("? VERIFICATION REUSSIE!")
        print("Tous les fichiers sont presents et l'environnement est pret.")
        print()
        print("Pour executer les tests:")
        print("  pytest")
        print("  OU")
        print("  .\\run_tests.ps1")
    else:
        print("? VERIFICATION ECHOUEE")
        print("Certains fichiers ou packages sont manquants.")
        print("Installez les packages avec: pip install -r requirements.txt")
    print("="*60)
    
    return 0 if all_ok else 1

if __name__ == "__main__":
    sys.exit(main())
