import { chromium } from "playwright"
import auth from "./spec/auth"

export async function mochaGlobalSetup() {
    let browser = await chromium.launch({ headless: false })
    await auth.admin({ browser })
    await auth.reseller({ browser })
    await browser.close()
}

export async function mochaGlobalTeardown() {
    await browser.close()
}
