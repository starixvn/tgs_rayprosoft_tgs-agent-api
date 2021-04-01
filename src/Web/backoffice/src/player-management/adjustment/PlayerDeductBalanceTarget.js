import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Dialog,
    useModalState,
    Stack,
    Heading,
    Text,
    Box,
    Set,
    FieldStack
} from "bumbag"
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

export const DeductBalanceTargetSelect = ({
    placeholder,
    placeholderDisabled = false,
    ...props
}) => {
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

    let defaultPlaceholder = useTrans({
        zh: "对象",
        en: "Target"
    })
    placeholder = placeholder ?? defaultPlaceholder
    if (placeholder) {
        options = [
            {
                label: placeholder,
                value: "",
                disabled: placeholderDisabled
            },
            ...options
        ]
    }

    return <Select options={options} {...props} />
}

export default {
    Player,
    Agent: Player
}
