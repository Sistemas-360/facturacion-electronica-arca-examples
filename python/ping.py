import json

from config import api_request


def main() -> None:
    response = api_request("GET", "/api/ping")
    print(json.dumps(response.json(), indent=2, ensure_ascii=False))


if __name__ == "__main__":
    main()
