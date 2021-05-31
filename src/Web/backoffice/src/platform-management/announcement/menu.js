import React from "react"
import { Locales, getTrans } from "../../i18n"

export const Code = "announcement"

export const Create = "create"

export const Update = "update"

export const Verify = "verify"

export const View = "view"

export const label = ({ codes = [], locale = Locales.zh }) => {
    let [code = ""] = codes
    code = code.toLowerCase()

    switch (code) {
        case Create:
            return getTrans(
                {
                    zh: "新增",
                    en: "Add"
                },
                locale
            )
        case Update:
            return getTrans(
                {
                    zh: "修改",
                    en: "Edit"
                },
                locale
            )
        case Verify:
            return getTrans(
                {
                    zh: "审核",
                    en: "Audit"
                },
                locale
            )
        case View:
            return getTrans(
                {
                    zh: "查看",
                    en: "View"
                },
                locale
            )
        default:
            return ""
    }
}

export const codeLabelPair = [Code, label]
