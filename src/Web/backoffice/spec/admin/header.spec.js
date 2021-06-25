import { chromium } from "playwright"
import { expect } from "chai"
import { StorageState, UserName } from "../auth"
import { host, headless } from "../env"

let browser
let context
let page

describe("Admin user header", () => {
    before(async () => {
        browser = await chromium.launch({ headless: headless() })
        context = await browser.newContext({ storageState: StorageState.Admin })
        page = await context.newPage()

        await page.goto(host(), {
            waitUntil: "networkidle0"
        })
    })

    after(async () => {
        if (!page.isClosed()) {
            await browser.close()
        }
    })

    it("should display user name", async () => {
        let username = await page.$eval(
            "#app-header #username",
            ({ textContent }) => textContent
        )

        expect(username).to.equal(UserName.Admin)
    })
})
