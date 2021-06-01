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
    Popover,
    usePopoverState
} from "bumbag"
import { faClipboard } from "@fortawesome/free-solid-svg-icons"
import copy from "clipboard-copy"
import { Trans } from "../i18n"

export const CopyTextBox = ({
    width = "100%",
    overflow = "hidden",
    textOverflow = "ellipsis",
    value = "",
    ...props
}) => {
    let popover = usePopoverState()
    return (
        <Box>
            <Popover.Disclosure
                {...popover}
                onClick={() => copy(value)}
                width={width}
                overflow={overflow}
                textOverflow={textOverflow}
                {...props}
            >
                <span title={value}>{value}</span>
            </Popover.Disclosure>
            {popover.visible && (
                <Popover
                    hasArrow
                    maxWidth="90vw"
                    whiteSpace="normal"
                    {...popover}
                >
                    <Trans zh={`复制了 ${value}`} en={`${value} Copied`} />
                </Popover>
            )}
        </Box>
    )
}
