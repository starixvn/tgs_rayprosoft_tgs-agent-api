import React from "react"
import { Locales, getTrans } from "../../i18n"

export const Code = "brands"

export const View = "view"

export const Update = "update"

export const label = ({ codes = [], locale = Locales.zh }) => {
    let [code = ""] = codes
    code = code.toLowerCase()

    switch (code) {
        case View:
            return getTrans(
                {
                    zh: "查看",
                    en: "View"
                },
                locale
            )
        case Update:
            return getTrans(
                {
                    zh: "编辑",
                    en: "Edit"
                },
                locale
            )
        default:
            return ""
    }
}

export const codeLabelPair = [Code, label]
