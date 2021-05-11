import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans } from "../../i18n"
import Select from "../../select/Select"
import { usePlayer } from "../player-list/client"

export const Bonus = "Bonus"
const BonusTrans = {
    zh: "红利",
    en: "Bonus"
}

export const Deposit = "Recharge"
const DepositTrans = {
    zh: "充值",
    en: "Deposit"
}

export const Transfer = "Transfer"
const TransferTrans = {
    zh: "转账",
    en: "Transfer"
}

export const Other = "Other"
const OtherTrans = {
    zh: "其他",
    en: "Other"
}

export const PlayerDeductBalanceTypeSelect = ({
    placeholder,
    playerCode,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "交易类型",
        en: "Transaction Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(BonusTrans),
            value: Bonus
        },
        {
            label: useTrans(DepositTrans),
            value: Deposit
        }
    ]

    let transferOption = {
        label: useTrans(TransferTrans),
        value: Transfer
    }
    let otherOption = {
        label: useTrans(OtherTrans),
        value: Other
    }

    let { isLoading, data: player, error } = usePlayer(playerCode)

    if (player?.agentUsername) {
        options.push(transferOption)
    }

    options.push(otherOption)

    return <Select placeholder={placeholder} options={options} {...props} />
}

export default {
    Bonus,
    Deposit,
    Transfer,
    Other
}
