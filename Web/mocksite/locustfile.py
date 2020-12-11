from locust import HttpUser, task, between

class MocksiteUser(HttpUser):
    @task
    def index(self):
        self.client.get("/")
        self.client.get("/favicon.ico")
        self.client.get("/web_modules/react.js")
        self.client.get("/web_modules/react-dom.js")
        self.client.get("/web_modules/react-router-dom.js")
        self.client.get("/web_modules/swr.js")
        self.client.get("/web_modules/bumbag.js")
        self.client.get("/web_modules/react-hook-form.js")
        self.client.get("/web_modules/@emotion/react.js")

        # response = self.client.post("/api/auth/jwt", {"client":"xdZYrxH3X8xXn2KlhWqB3VB4TjZrHfp4rM5xbHfa8cU3oz6CZDL3HqcuZ07pPQFV","currency":"CNY"})
        # print("/api/auth/jwt response status code:", response.status_code)
        # print("/api/auth/jwt response text:", response.text)

    wait_time = between(0.5, 10)