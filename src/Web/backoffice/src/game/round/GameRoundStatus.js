import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useTrans, Trans } from "../../i18n"
import Select from "../../select/Select"

const Win = "Win"
const WinTrans = {
    zh: "赢",
    en: "Win"
}

const Lose = "Loss"
const LoseTrans = {
    zh: "输",
    en: "Loss"
}

const Draw = "Draw"
const DrawTrans = {
    zh: "和",
    en: "Draw"
}

const Open = "Open"
const OpenTrans = {
    zh: "未结算",
    en: "Open"
}

export const GameRoundStatus = {
    Win,
    Lose,
    Draw,
    Open
}

export const GameRoundStatusLabel = ({ value }) => {
    switch (value) {
        case Win:
            return <Trans {...WinTrans} />

        case Lose:
            return <Trans {...LoseTrans} />

        case Draw:
            return <Trans {...DrawTrans} />

        case Open:
            return <Trans {...OpenTrans} />

        default:
            return value
    }
}

export const GameRoundStatusSelect = ({ placeholder, ...props }) => {
    let options = [
        {
            value: Win,
            label: useTrans(WinTrans)
        },
        {
            value: Lose,
            label: useTrans(LoseTrans)
        },
        {
            value: Draw,
            label: useTrans(DrawTrans)
        },
        {
            value: Open,
            label: useTrans(OpenTrans)
        }
    ]

    let defaultPlaceholder = useTrans({
        zh: "状态",
        en: "Status"
    })
    placeholder = placeholder ?? defaultPlaceholder

    return <Select options={options} placeholder={placeholder} {...props} />
}

export default GameRoundStatus
