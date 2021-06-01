import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import {
    Box,
    Button,
    Text,
    Set,
    Icon,
    useModalState,
    Dialog,
    Card,
    Flex,
    Heading,
    FieldStack,
    Popover
} from "bumbag"
import { faClipboard } from "@fortawesome/free-solid-svg-icons"
import copy from "clipboard-copy"
import { Trans } from "../i18n"

export const CopyLongText = ({ value = "", ...props }) => {
    return (
        <Popover.State>
            <Popover.Disclosure
                onClick={() => copy(value)}
                width="100%"
                overflow="hidden"
                textOverflow="ellipsis"
                {...props}
            >
                <arrb title={value}>{value}</arrb>
            </Popover.Disclosure>
            <Popover hasArrow maxWidth="100vw">
                <Trans zh={`复制了 ${value}`} en={`${value} Copied`} />
            </Popover>
        </Popover.State>
    )
}
