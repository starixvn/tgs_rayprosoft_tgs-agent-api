import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import {
    Code as AgentListCode,
    codeLabelPair as agentListPair
} from "./agent-list/menu"
import {
    Code as BalanceAdjustmentCode,
    codeLabelPair as balanceAdjustmentListPair
} from "./balance-adjustment/menu"
import { getTrans, useTrans } from "../i18n"

export const Name = ({}) => useTrans(nameMessages)

export const nameMessages = {
    zh: "代理管理",
    en: "Agent management"
}

export const Code = "agentmanagement"

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
        case AgentListCode:
            return getTrans(
                {
                    zh: "代理列表",
                    en: "Agents"
                },
                locale
            )

        case BalanceAdjustmentCode:
            return getTrans(
                {
                    zh: "代理调账",
                    en: "Balance Adjustment"
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
            subLabelPairs: [agentListPair, balanceAdjustmentListPair]
        })
]
