import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import {
    Code as AdminLogCode,
    codeLabelPair as adminLogPair
} from "./admin/menu"
import { Code as ApiLogCode, codeLabelPair as apiLogPair } from "./api/menu"
import { Code as LogCode, codeLabelPair as logPair } from "./log/menu"
import { getTrans, useTrans } from "../i18n"

export const Name = ({}) => useTrans(nameMessages)

export const nameMessages = {
    zh: "Log",
    en: "Log"
}

export const Code = "log"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

export const Menu = ({
    title = <Name />,
    url = DefaultUrl,
    codes,
    codePaths = DefaultCodePath,
    ...props
}) => (
    <_Menu
        title={title}
        url={url}
        codes={codes}
        codePaths={codePaths}
        itemLabel={label}
    />
)

const label = ({ locale, code }) => {
    switch (code) {
        case AdminLogCode:
            return getTrans(
                {
                    zh: "Admin Log",
                    en: "Admin Log"
                },
                locale
            )

        case ApiLogCode:
            return getTrans(
                {
                    zh: "API Log",
                    en: "API Log"
                },
                locale
            )

        case LogCode:
            return getTrans(
                {
                    zh: "Log",
                    en: "Log"
                },
                locale
            )
        default:
            return ""
    }
}

export const codeLabelPair = [
    Code,
    ({ locale, codes }) =>
        findlLabel({
            locale,
            codes,
            label,
            subLabelPairs: [adminLogPair, apiLogPair, logPair]
        })
]
