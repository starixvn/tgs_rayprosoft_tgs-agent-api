import React, { useRef, useEffect } from "react"
import { css } from "@emotion/react"
import { Menu as _Menu, findlLabel } from "../menu"
import {
    Code as PromotionListCode,
    codeLabelPair as promotionListPair
} from "./promotion-list/menu"
import {
    Code as PayoutListCode,
    codeLabelPair as payoutListPair
} from "./payout-list/menu"
import { getTrans, useTrans } from "../i18n"

export const Code = "promotion"

export const DefaultUrl = `/${Code}`

export const DefaultCodePath = [Code]

export const Name = ({}) => useTrans(nameMessages)

export const nameMessages = {
    zh: "活动管理",
    en: "Promotion Management"
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
        case PromotionListCode:
            return getTrans(
                {
                    zh: "活动管理",
                    en: "Promotions"
                },
                locale
            )
        case PayoutListCode:
            return getTrans(
                {
                    zh: "活动派发列表",
                    en: "Promotion Payouts"
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
            subLabelPairs: [promotionListPair, payoutListPair]
        })
]
