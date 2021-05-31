import React from "react"
import { getTrans } from "../../i18n"

export const Code = "departments"

export const View = "view"

export const Create = "create"

export const Update = "update"

export const Delete = "delete"

export const label = (codes = [], locale = Locales.zh) => {
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
                    zh: "编辑",
                    en: "Edit"
                },
                locale
            )
        case Delete:
            return getTrans(
                {
                    zh: "删除",
                    en: "Delete"
                },
                locale
            )
        default:
            return ""
    }
}

export const codeLabelPair = [Code, ({ codes, locale }) => label(codes, locale)]
