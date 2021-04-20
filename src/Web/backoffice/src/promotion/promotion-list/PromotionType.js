import React from "react"
import { Spinner } from "bumbag"
import { getTrans, Trans, useLocale, useTrans } from "../../i18n"
import Select from "../../select/Select"
import { usePromotionTypeList } from "./client"

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

const Deposit = "Deposit"
const DepositTrans = {
    zh: "充值送",
    en: "Deposit Rebate"
}

const TurnoverRebate = "TurnoverRebate"
const TurnoverRebateTrans = {
    zh: "流水反水",
    en: "Turnover Rebate"
}

const Turnover = "Turnover"
const TurnoverTrans = {
    zh: "流水",
    en: "Turnover"
}

const ReliefMoneyDaily = "ReliefMoneyDaily"
const ReliefMoneyDailyTrans = {
    zh: "每日救援金",
    en: "Daily Relief Money"
}

const ReliefMoney = "ReliefMoney"
const ReliefMoneyTrans = {
    zh: "救援金",
    en: "Relief Money"
}

const AllTrans = [
    {
        name: Deposit,
        trans: DepositTrans,
        types: [
            {
                name: FirstDeposit,
                trans: FirstDepositTrans
            },
            {
                name: SecondDeposit,
                trans: SecondDepositTrans
            }
        ]
    },
    {
        name: Turnover,
        trans: TurnoverTrans,
        types: [
            {
                name: TurnoverRebate,
                trans: TurnoverRebateTrans
            }
        ]
    },
    {
        name: ReliefMoney,
        trans: ReliefMoneyTrans,
        types: [
            {
                name: ReliefMoneyDaily,
                trans: ReliefMoneyDailyTrans
            }
        ]
    }
]

const findTrans = (name, list = AllTrans) => {
    let result = list.reduce((result, group) => {
        if (result) {
            return result
        }

        if (group.name === name) {
            return group.trans
        }

        if (group.types) {
            result = findTrans(name, group.types)
        }
        return result
    }, undefined)

    return result
}

const findGroupTrans = (name) => {
    let groups = AllTrans.map(({ types: _, ...group }) => group)

    return findTrans(name, groups)
}

const findGroup = (name, groups = []) =>
    groups.find((group) => group.name === name || group.Name === name)

export const usePromotionGroupLabelOf = (promotionType) => {
    let locale = useLocale()
    let { isLoading, data: groups, error } = usePromotionTypeList()
    if (isLoading) {
        return ""
    }

    let group = groups?.reduce((result, group) => {
        if (result) {
            return result
        }

        if (group?.types?.includes(promotionType)) {
            return group
        }
    }, undefined)

    let trans = findGroupTrans(group?.name)
    return trans?.[locale] ?? group?.name ?? ""
}

export const usePromotionGroupValueOf = (promotionType) => {
    let { isLoading, data: groups, error } = usePromotionTypeList()
    if (isLoading) {
        return ""
    }

    let group = groups?.reduce((result, group) => {
        if (result) {
            return result
        }

        if (group?.types?.includes(promotionType)) {
            return group
        }
    }, undefined)

    return group?.name ?? ""
}

/*
export const PromotionTypeGroupLabel = ({ value }) => {
    let locale = useLocale()
    let trans = findGroupTrans(value)

    return trans?.[locale] ?? value
}
*/

export const PromotionTypeGroupSelect = ({ placeholder, ...props }) => {
    let locale = useLocale()
    let defaultPlaceholder = useTrans({
        zh: "活动类型",
        en: "Promotion Type Group"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { isLoading, data: groups, error } = usePromotionTypeList()
    if (isLoading) {
        return <Spinner />
    }

    let options = groups?.map(({ name }) => {
        let trans = findGroupTrans(name)
        return {
            label: trans?.[locale] ?? name,
            value: name
        }
    })
    return <Select placeholder={placeholder} options={options} {...props} />
}

export const PromotionTypeLabel = ({ value }) => {
    let locale = useLocale()
    return findTrans(value)?.[locale] ?? value
}

export const PromotionTypeSelect = ({ placeholder, group, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "活动子类型",
        en: "Promotion Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let locale = useLocale()
    let { isLoading, data: groups, error } = usePromotionTypeList()
    let options = groups
        ?.filter(({ name }) => {
            if (group) {
                return group === name
            }
            return true
        })
        .reduce((list, { types = [], ...group }) => {
            let options = types.map((type) => {
                let label = findTrans(type)?.[locale] ?? type
                return {
                    label,
                    value: type
                }
            })

            return [...list, ...options]
        }, [])

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}
