import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Trans, useTrans } from "../../i18n"
import { Text } from "bumbag"
import { useVipLevelList } from "../../vip-levels/client"
import { RadioGroup } from "../../radio/RadioGroup"
import { Switch } from "../../switch/Switch"

export const All = "All"
const AllTrans = {
    zh: "满足所有",
    en: "All"
}

export const Someone = "Someone"
const SomeoneTrans = {
    zh: "满足其一",
    en: "Satisfy one"
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
        playerListCondition === Someone
            ? useTrans(SomeoneTrans)
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
        useVipLevelDetail(vipLevelIds)
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
        { label: useTrans(AllTrans), value: All },
        { label: useTrans(SomeoneTrans), value: Someone }
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
        case All:
            return <AllPlayerText />
        case Someone:
            return <Trans zh="特定玩家" en="Specific Players" />
    }
    return ""
}

export const VipLevelsText = () => <Trans {...VipLevelsTrans} />

export const useVipLevelDetail = (vipLevels = []) => {
    let vipLevel = useTrans(VipLevelsTrans)
    let { data } = useVipLevelList()
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
        let vipName = item ? `/VIP ${item.level}` : ""
        return detail ? `${detail}/${vipName}` : vipName
    }, "")

    return names ? `(${names})` : ""
}
