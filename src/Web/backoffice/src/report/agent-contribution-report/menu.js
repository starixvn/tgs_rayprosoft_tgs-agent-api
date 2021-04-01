import React from "react"
import { Locales, getTrans } from "../../i18n"

export const Code = "agentcontributionreport"

export const View = "view"

export const codeLabelPair = [Code, ({ codes, locale }) => label(codes, locale)]

export const label = (codes = [], locale = Locales.zh) => {
    let [code = ""] = codes
    code = code.toLowerCase()

    switch (code) {
        case View:
            return getTrans(
                {
                    zh: "查看报表",
                    en: "View report"
                },
                locale
            )
        default:
            return ""
    }
}
