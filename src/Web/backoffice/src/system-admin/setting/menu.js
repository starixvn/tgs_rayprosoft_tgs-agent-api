import React from "react"
import { getTrans } from "../../i18n"

export const Code = "settings"

export const Update = "update"

export const label = (codes = [], locale = Locales.zh) => {
    let [code = ""] = codes
    code = code.toLowerCase()

    switch (code) {
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

export const codeLabelPair = [Code, ({ codes, locale }) => label(codes, locale)]
