import React from "react"
import { getTrans } from "../../i18n"

export const Code = "brandgames"

export const View = "view"

export const Update = "update"

export const label = ({ codes, locale }) => {
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
            return getTrans({
                zh: "编辑",
                en: "Edit"
            })
        default:
            return ""
    }
}

export const codeLabelPair = [Code, label]
