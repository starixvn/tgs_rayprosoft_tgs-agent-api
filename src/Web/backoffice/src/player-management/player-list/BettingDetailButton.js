import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useLocation, useRouteMatch } from "react-router-dom"
import { Stack, Box, Text, Dialog } from "bumbag"
import Disclosure from "../../dialog/Disclosure"
import { Modal, useModalState } from "../../dialog/Modal"
import { Trans, useTrans } from "../../i18n"
import GameRoundDialog from "../../game/round/GameRoundDialog"
import Backdrop from "../../dialog/Backdrop"

export const Button = ({ record, ...props }) => {
    let dialog = useModalState()

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="详情" en="Detail" />
            </Disclosure>
            {dialog.visible && (
                <Backdrop {...dialog}>
                    <GameRoundDialog
                        record={record}
                        dialog={dialog}
                        hideBackdrop={true}
                    />
                </Backdrop>
            )}
        </>
    )
}

const titleTrans = {
    zh: "详情",
    en: "Detail"
}

export default Button
