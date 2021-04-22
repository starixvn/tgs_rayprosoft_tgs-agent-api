import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Trans, useTrans } from "../../i18n"
import { Text } from "bumbag"
import { useVipLevelList } from "../../vip-levels/client"
import { RadioGroup } from "../../radio/RadioGroup"
import { Switch } from "../../switch/Switch"

const MatchAll = "MatchAll"
const AllTrans = {
    zh: "满足所有",
    en: "Match All"
}

const MatchOne = "MatchOne"
const MatchOneTrans = {
    zh: "满足其一",
    en: "Satisfy One"
}

const AllPlayerTrans = {
    zh: "全部玩家",
    en: "All Players"
}

const AccumulatedDepositTrans = {
    zh: "累积存款",
    en: "Accumulated Deposit"
}

const UnlimitedTrans = {
    zh: "无上限",
    en: "Unlimited"
}

const IdentityAuthenticationTrans = {
    zh: "身份验证",
    en: "Identity Authentication"
}

const PhoneAuthenticationTrans = {
    zh: "手机验证",
    en: "Phone Authentication"
}

const VipLevelsTrans = {
    zh: "VIP群",
    en: "VIP Levels"
}

export const Detail = ({ record, whiteSpace = "nowrap", ...props }) => {
    let {
        brandId,
        playerListCondition,
        allPlayers,
        depositedAmountMin,
        depositedAmountMax,
        depositedAmountUnlimited,
        identityAuthentication,
        phoneAuthentication,
        vipLevelIds = []
    } = record

    let condition =
        playerListCondition === MatchOne
            ? useTrans(MatchOneTrans)
            : useTrans(AllTrans)

    let details = [
        allPlayers ? useTrans(AllPlayerTrans) : "",
        `${useTrans(AccumulatedDepositTrans)} ${depositedAmountMin} - ${
            depositedAmountUnlimited
                ? useTrans(UnlimitedTrans)
                : depositedAmountMax
        }`,
        identityAuthentication ? useTrans(IdentityAuthenticationTrans) : "",
        phoneAuthentication ? useTrans(PhoneAuthenticationTrans) : "",
        useVipLevelDetail(brandId, vipLevelIds)
    ].filter((string) => !!string)

    return (
        <Text.Block
            whiteSpace={whiteSpace}
            {...props}
        >{`${condition}: ${details.join(", ")}`}</Text.Block>
    )
}

export const PlayerListConditionRadioGroup = ({
    name = "playerListCondition",
    ...props
}) => {
    let options = [
        { label: useTrans(AllTrans), value: MatchAll },
        { label: useTrans(MatchOneTrans), value: MatchOne }
    ]

    return <RadioGroup name={name} options={options} {...props} />
}

export const AllPlayerText = () => <Trans {...AllPlayerTrans} />

export const UnlimitedText = () => <Trans {...UnlimitedTrans} />

export const AccumulatedDepositText = () => (
    <Trans {...AccumulatedDepositTrans} />
)

export const IdentityAuthenticationText = () => (
    <Trans {...IdentityAuthenticationTrans} />
)

export const PhoneAuthenticationText = () => (
    <Trans {...PhoneAuthenticationTrans} />
)

export const PlayerLabel = ({ value }) => {
    switch (value) {
        case MatchAll:
            return <AllPlayerText />
        case MatchOne:
            return <Trans zh="特定玩家" en="Specific Players" />
    }
    return ""
}

export const VipLevelsText = () => <Trans {...VipLevelsTrans} />

export const useVipLevelDetail = (brandId, vipLevels = []) => {
    let vipLevel = useTrans(VipLevelsTrans)
    let { data } = useVipLevelList({ brandId, showAll: true })
    if (vipLevels.length > 0) {
        return `${vipLevel}${toVipLevelNames(vipLevels, data?.items)}`
    }
    return ""
}

const toVipLevelNames = (ids = [], list = []) => {
    if (list.length === 0 || ids.length === 0) {
        return ""
    }

    let names = ids.reduce((detail, id) => {
        let item = list.find((item) => item.id === id)
        if (!item) {
            console.warn(`VIP id [${id}] not found`)
            return detail
        }
        return detail ? `${detail}/${item.name}` : item.name
    }, "")

    return names ? `(${names})` : ""
}
