import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import {
    Code as SettingsCode,
    codeLabelPair as settingsPair
} from "./setting/menu"
import { getTrans, useTrans } from "../i18n"

export const Code = "systemadmin"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

export const Name = ({}) => useTrans(nameMessages)

export const nameMessages = {
    zh: "系统管理",
    en: "System Admin"
}

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
        itemLabel={labelOf}
    />
)

const labelOf = ({ locale, code }) => {
    switch (code) {
        case SettingsCode:
            return getTrans(
                {
                    zh: "设定",
                    en: "Settings"
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
            label: labelOf,
            subLabelPairs: [settingsPair]
        })
]
