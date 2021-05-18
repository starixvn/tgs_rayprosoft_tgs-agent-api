import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import {
    Code as GameProviderCode,
    codeLabelPair as gameProviderPair
} from "./game-provider/menu"
import {
    Code as LanguageCode,
    codeLabelPair as languagePair
} from "./language/menu"
import {
    Code as CurrencyCode,
    codeLabelPair as currencyPair
} from "./currency/menu"
import {
    Code as BetLimitCode,
    codeLabelPair as betLimitPair
} from "./betlimit/menu"
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
        case GameProviderCode:
            return getTrans(
                {
                    zh: "游戏商管理",
                    en: "Game Provider Management"
                },
                locale
            )
        case LanguageCode:
            return getTrans(
                {
                    zh: "语系",
                    en: "Language"
                },
                locale
            )
        case CurrencyCode:
            return getTrans(
                {
                    zh: "币种",
                    en: "Currency"
                },
                locale
            )
        case BetLimitCode:
            return getTrans(
                {
                    zh: "限红",
                    en: "Bet Limit"
                },
                locale
            )
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
            subLabelPairs: [
                gameProviderPair,
                languagePair,
                currencyPair,
                betLimitPair,
                settingsPair
            ]
        })
]
