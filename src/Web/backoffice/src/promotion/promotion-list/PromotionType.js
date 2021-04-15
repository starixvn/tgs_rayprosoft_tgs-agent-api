import React from "react"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import Select from "../../select/Select"

const FirstDeposit = "FirstDeposit"
const FirstDepositTrans = {
    zh: "每日首充",
    en: "First Deposit"
}

const SecondDeposit = "SecondDeposit"
const SecondDepositTrans = {
    zh: "每日再充",
    en: "Second Deposit"
}

const DepositGroup = "DepositGroup"
const DepositGroupItems = [FirstDeposit, SecondDeposit]
const DepositGroupTrans = {
    zh: "充值送",
    en: "Deposit Rebate"
}

const Rebate = "Rebate"
const RebateTrans = {
    zh: "流水反水",
    en: "Turnover Rebate"
}

const TurnoverGroup = "TurnoverGroup"
const TurnoverGroupItems = [Rebate]
const TurnoverGroupTrans = {
    zh: "流水",
    en: "Turnover"
}

const ReliefMoney = "ReliefMoney"
const ReliefMoneyTrans = {
    zh: "每日救援金",
    en: "Daily Relief Money"
}

const ReliefGroup = "ReliefGroup"
const ReliefGroupItems = [ReliefMoney]
const ReliefGroupTrans = {
    zh: "救援金",
    en: "Relief Money"
}

const GroupItems = {
    DepositGroup: DepositGroupItems,
    TurnoverGroup: TurnoverGroupItems,
    ReliefGroup: ReliefGroupItems
}

const Trans = {
    FirstDeposit: FirstDepositTrans,
    SecondDeposit: SecondDepositTrans,
    Rebate: RebateTrans,
    ReliefMoney: ReliefMoneyTrans
}

export const PromotionTypeGroupLabel = ({ value }) => {
    switch (value) {
        case DepositGroup:
            return <Trans {...DepositGroupTrans} />
        case TurnoverGroup:
            return <Trans {...TurnoverGroupTrans} />
        case ReliefGroup:
            return <Trans {...ReliefGroupTrans} />
        default:
            return value
    }
}

export const PromotionGroupSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "活动类型",
        en: "Promotion Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(DepositGroupTrans),
            value: DepositGroup
        },
        {
            label: useTrans(TurnoverGroupTrans),
            value: TurnoverGroup
        },
        {
            label: useTrans(ReliefGroupTrans),
            value: ReliefGroup
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

export const PromotionTypeLabel = ({ value }) => {
    let locale = useLocale()
    let trans = toTrans(value)
    if (!trans) {
        return value
    }

    return trans[locale]
}

export const PromotionTypeSelect = ({ group, props }) => {
    let defaultPlaceholder = useTrans({
        zh: "活动子类型",
        en: "Promotion Subtype"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let items = toTypes(group)
    let locale = useLocale()
    let options = toOptions({ items, locale })

    return <Select options={options} {...props} />
}

const toTypes = (group = "") => GroupItems[group] ?? []

const toTrans = (type = "") => Trans[type]

const toOptions = ({ items = [], locale }) => {
    return items
        .map((item) => {
            let trans = toTrans(item)
            if (trans) {
                return {
                    label: trans[locale],
                    value: item
                }
            }
        })
        .filter((item) => !!item)
}
