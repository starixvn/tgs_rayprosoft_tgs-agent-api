import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useTrans } from "../../i18n"
import Select from "../../select/Select"

export const Player = "Player"
const PlayerTrans = {
    zh: "玩家",
    en: "Player"
}

export const Player = "Agent"
const AgentTrans = {
    zh: "代理",
    en: "Agent"
}

export const DeductBalanceTargetSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "对象",
        en: "Target"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(PlayerTrans),
            value: Player
        },
        {
            label: useTrans(AgentTrans),
            value: AgentTrans
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

export default {
    Player,
    Agent: Player
}
