import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Box, Dialog, useModalState } from "bumbag"
import { ViewDisclosure } from "../../dialog/Disclosure"
import GameRoundDialog from "../../game/round/GameRoundDialog"

export const GameRoundButton = ({ record }) => {
    let dialog = useModalState()

    return (
        <>
            <ViewDisclosure {...dialog} />
            {dialog.visible && (
                <GameRoundDialog record={record} dialog={dialog} />
            )}
        </>
    )
}
