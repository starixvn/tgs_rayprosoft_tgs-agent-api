import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Trans, useTrans } from "../../i18n"

const Player = "Player"
const PlayerTrans = {
    zh: "玩家",
    en: "Player"
}

const Agent = "Agent"
const AgentTrans = {
    zh: "代理",
    en: "Agent"
}

export const UserTypeLabel = ({ value = "" }) => {
    switch (value) {
        case Player:
            return <Trans {...PlayerTrans} />
        case Agent:
            return <Trans {...AgentTrans} />
        default:
            return value
    }
}
