import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import {
    Code as MarketReportCode,
    codeLabelPair as marketReportPair
} from "./market-report/menu"
import {
    Code as OrderReportCode,
    codeLabelPair as orderReportPair
} from "./order-report/menu"
import {
    Code as AgentContributionReportCode,
    codeLabelPair as agentContributionReportPair
} from "./agent-contribution-report/menu"
import {
    Code as PlayerContributionReportCode,
    codeLabelPair as playerContributionReportPair
} from "./player-contribution-report/menu"
import {
    Code as AgentBonusReportCode,
    codeLabelPair as agentBonusReportPair
} from "./agent-bonus-report/menu"
import {
    Code as VipReportCode,
    codeLabelPair as vipReportPair
} from "./vip-report/menu"
import { getTrans, useTrans, Trans } from "../i18n"

export const Code = "report"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

export const nameMessages = {
    zh: "报表管理",
    en: "Report Management"
}

export const Menu = ({
    title = <Trans {...nameMessages} />,
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
        case MarketReportCode:
            return getTrans(
                {
                    zh: "市场报表",
                    en: "Market Report"
                },
                locale
            )
        case OrderReportCode:
            return getTrans(
                {
                    zh: "订单查询",
                    en: "Order Tracking"
                },
                locale
            )

        case AgentContributionReportCode:
            return getTrans(
                {
                    zh: "代理贡献报表",
                    en: "Agent Contribution Report"
                },
                locale
            )

        case PlayerContributionReportCode:
            return getTrans(
                {
                    zh: "玩家贡献报表",
                    en: "Player Contribution Report"
                },
                locale
            )

        case AgentBonusReportCode:
            return getTrans(
                {
                    zh: "代理分红报表",
                    en: "Agent Profit Sharing Report"
                },
                locale
            )

        case VipReportCode:
            return getTrans(
                {
                    zh: "VIP报表",
                    en: "VIP Report"
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
            subLabelPairs: [
                marketReportPair,
                orderReportPair,
                agentContributionReportPair,
                playerContributionReportPair,
                agentBonusReportPair,
                vipReportPair
            ]
        })
]
