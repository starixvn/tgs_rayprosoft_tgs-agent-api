import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Text } from "bumbag"
import { useTrans, Trans } from "../i18n"
import Select from "../select/Select"

const Chief = "Chief"
const ChiefTrans = {
    zh: "总代",
    en: "Chief"
}

const Agent = "Agent"
const AgentTrans = {
    zh: "代理",
    en: "Agent"
}

export default {
    Chief,
    Agent
}

export const isAgentHead = (value) => value === Chief

export const isAgentParentRequired = (value) => !isAgentHead(value)

export const AgentLevelSelect = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "代理种类",
        en: "Level Type"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let options = [
        {
            label: useTrans(ChiefTrans),
            value: Chief
        },
        {
            label: useTrans(AgentTrans),
            value: Agent
        }
    ]

    return <Select placeholder={placeholder} options={options} {...props} />
}

export const AgentLevelLabel = ({ value = "" }) => {
    let code = value
    switch (code) {
        case Chief:
            return (
                <Text>
                    <Trans {...ChiefTrans} />
                </Text>
            )
        case Agent:
            return (
                <Text>
                    <Trans {...AgentTrans} />
                </Text>
            )
        default:
            return value
    }
}
