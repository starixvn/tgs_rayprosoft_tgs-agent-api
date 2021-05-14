import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useModalState, Dialog, Text, Box, List } from "bumbag"
import { ViewDisclosure } from "../dialog/Disclosure"
import { Locales, Trans, useTrans } from "../i18n"
import { useVipLevelList } from "./client"
import LoadingBackdrop from "../dialog/LoadingBackdrop"
import AlertDialog from "../dialog/Alert"

export const VipLevelLabel = ({ value, placeholder = "-", ...props }) => {
    if (Number.isInteger(value)) {
        return `VIP ${value}`
    }

    return value ?? placeholder
}
