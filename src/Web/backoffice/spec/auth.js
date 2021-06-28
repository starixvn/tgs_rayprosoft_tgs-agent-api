import { expect } from "chai"
import { host } from "./env"

const Path = "./.playwright"

const AdminAuthPath = `${Path}/admin-auth.json`
const AdminUserName = "admin.enabled"
const admin = async ({ browser }) =>
    login({
        username: AdminUserName,
        password: "TgsP@ssw0rd",
        browser,
        storageStatePath: AdminAuthPath
    })

const ResellerAuthPath = `${Path}/reseller-auth.json`
const ResellerUserName = "reseller.enabled"
const reseller = async ({ browser }) =>
    login({
        username: ResellerUserName,
        password: "TgsP@ssw0rd",
        browser,
        storageStatePath: ResellerAuthPath
    })

const login = async ({
    browser,
    storageStatePath,
    username = "",
    password = "TgsP@ssw0rd"
}) => {
    let context = await browser.newContext()
    let page = await context.newPage()
    await page.goto(`${host()}/login`, {
        waitUntil: "networkidle0"
    })

    let userNameInputSelector = `form input[name='username']`
    await page.waitForSelector(userNameInputSelector)

    await page.click(userNameInputSelector)
    await page.type(userNameInputSelector, username)

    let passwordInputSelectot = `form input[name="password"]`
    await page.click(passwordInputSelectot)
    await page.type(passwordInputSelectot, password)

    let [loginRequest, loginResponse] = await Promise.all([
        page.waitForRequest((request) => {
            return (
                request.url().endsWith("/api/auth") &&
                request.method() === "POST"
            )
        }),
        page.waitForResponse((response) => {
            return (
                response.url().endsWith("/api/auth") &&
                response.status() === 200
            )
        }),
        page.click(`form button[type="submit"]`)
    ])

    let response = await loginResponse.json()
    expect(response.valid).to.be.true
    expect(response).to.have.property("userInfo")

    await context.storageState({ path: storageStatePath })

    if (!page.isClosed()) {
        await page.close()
    }
}

export const StorageState = {
    Admin: AdminAuthPath,
    Reseller: ResellerAuthPath
}

export const UserName = {
    Admin: AdminUserName,
    Reseller: ResellerUserName
}

export default {
    admin,
    reseller,
    StorageState,
    UserName
}
