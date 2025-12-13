# -*- coding: utf-8 -*-
"""
Python Integration Tests Runner
Run this script to execute integration tests with proper setup checks
"""
import sys
import subprocess
import requests
import time
from pathlib import Path

def print_header(text):
    """Print a formatted header"""
    print("\n" + "=" * 60)
    print(f"  {text}")
    print("=" * 60)

def check_api_running(api_url):
    """Check if the Backend API is running"""
    try:
        response = requests.get(f"{api_url}/api/vehicles", timeout=2)
        return True
    except (requests.exceptions.ConnectionError, requests.exceptions.Timeout):
        return False

def check_frontend_running(base_url):
    """Check if the Frontend is running"""
    try:
        response = requests.get(base_url, timeout=2)
        return True
    except (requests.exceptions.ConnectionError, requests.exceptions.Timeout):
        return False

def main():
    """Main test runner"""
    print_header("Python Integration Tests - Pre-flight Check")
    
    # Check if we're in the right directory
    if not Path("conftest.py").exists():
        print("❌ ERROR: Must run from IntegrationTests directory")
        print("   Run: cd IntegrationTests")
        sys.exit(1)
    
    # Configuration
    API_URL = "http://localhost:5001"
    BASE_URL = "http://localhost:5000"
    
    # Check API
    print("\n1. Checking Backend API...")
    if check_api_running(API_URL):
        print(f"   ✅ Backend API is running at {API_URL}")
    else:
        print(f"   ❌ Backend API is NOT running at {API_URL}")
        print("   Please start the Backend:")
        print("   - Open terminal in Backend folder")
        print("   - Run: dotnet run")
        print("\n   Note: Some tests will be skipped without the API")
    
    # Check Frontend (optional for API tests)
    print("\n2. Checking Frontend...")
    if check_frontend_running(BASE_URL):
        print(f"   ✅ Frontend is running at {BASE_URL}")
    else:
        print(f"   ⚠️  Frontend is NOT running at {BASE_URL}")
        print("   UI tests will be skipped. To run UI tests:")
        print("   - Open terminal in Frontend folder")
        print("   - Run: dotnet run")
    
    # Check dependencies
    print("\n3. Checking Python dependencies...")
    try:
        import pytest
        import selenium
        print("   ✅ All dependencies installed")
    except ImportError as e:
        print(f"   ❌ Missing dependency: {e}")
        print("   Run: pip install -r requirements.txt")
        sys.exit(1)
    
    # Ask user what to run
    print_header("Test Execution Options")
    print("\n1. Run ALL tests (API + UI)")
    print("2. Run API tests only")
    print("3. Run UI tests only")
    print("4. Run with HTML report")
    print("5. Exit")
    
    choice = input("\nSelect option (1-5): ").strip()
    
    # Build pytest command
    pytest_args = ["-v", "--tb=short"]
    
    if choice == "1":
        print_header("Running ALL Tests")
        # No filter, run all
    elif choice == "2":
        print_header("Running API Tests Only")
        pytest_args.extend(["-m", "api"])
    elif choice == "3":
        print_header("Running UI Tests Only")
        pytest_args.extend(["-m", "ui"])
    elif choice == "4":
        print_header("Running ALL Tests with HTML Report")
        pytest_args.extend(["--html=reports/report.html", "--self-contained-html"])
    elif choice == "5":
        print("Goodbye!")
        sys.exit(0)
    else:
        print("Invalid choice. Running all tests...")
    
    # Run pytest
    print("\n" + "-" * 60)
    result = subprocess.run(["pytest"] + pytest_args)
    
    # Summary
    print_header("Test Execution Complete")
    if result.returncode == 0:
        print("✅ All tests passed!")
    else:
        print("❌ Some tests failed or were skipped")
        print("\nTroubleshooting:")
        print("- Make sure Backend is running: cd Backend && dotnet run")
        print("- Make sure Frontend is running (for UI tests): cd Frontend && dotnet run")
        print("- Check logs above for specific errors")
    
    return result.returncode

if __name__ == "__main__":
    try:
        sys.exit(main())
    except KeyboardInterrupt:
        print("\n\n⚠️  Tests interrupted by user")
        sys.exit(1)
