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
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure from "../../dialog/Disclosure"
import { FormattedNumberDecimal, Trans, useTrans } from "../../i18n"
import {
    CloseButtonFooter,
    ConfirmButtonFooter
} from "../../form/ConfirmButtonFooter"
import { Table, toApiParams, useTable, pageCount } from "../../table"
import { useAgentWinLossList } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import dateTimeColumn from "../../table/column/dateTime"
import {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import textColumn from "../../table/column/textColumn"
import AlertDialog from "../../dialog/Alert"

export const AgentWinLossDialogDisclosure = ({
    brandId,
    username,
    start,
    end,
    value,
    ...props
}) => {
    let dialog = useModalState()

    return (
        <>
            <Disclosure {...dialog} {...props}>
                <FormattedNumberDecimal value={value} />
            </Disclosure>
            {dialog.visible && (
                <ReportDialog
                    brandId={brandId}
                    username={username}
                    winLossAmount={value}
                    start={start}
                    end={end}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ReportDialog = ({
    brandId,
    username,
    winLossAmount,
    start,
    end,
    dialog
}) => {
    let { state, setPageIndex, setSortOrder } = useTable({
        brandId,
        /*
        username,
        */
        from: start,
        to: end,
        showAll: true
    })

    let { data, isLoading, error, mutate, deleteCache } = useAgentWinLossList(
        username,
        toApiParams(state)
    )

    let columns = useAgentPlayerWinLossColumns({ items: data?.items })

    let title = useTrans({
        zh: "输赢详情",
        en: "Win Loss Detail"
    })

    if (isLoading) {
        return <LoadingBackdrop {...dialog} />
    }

    if (error) {
        return <AlertDialog {...dialog} />
    }

    return (
        <Dialog.Modal
            title={title}
            hideOnClickOutside={false}
            {...dialog}
            showCloseButton
            placement="top"
            showActionButtons={false}
            footer={<CloseButtonFooter dialog={dialog} />}
            maxWidth="90vw"
            maxHeight="90vh"
        >
            <Stack>
                <Table
                    controlledState={state}
                    columns={columns}
                    data={data?.items}
                    // pageCount={pageCount(pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    isLoading={isLoading}
                    foot={data?.items?.length > 0}
                />
            </Stack>
        </Dialog.Modal>
    )
}

const useAgentPlayerWinLossColumns = ({ items = [] }) => {
    return useMemo(
        () => [
            textColumn({
                accessor: "username",
                Header: <Trans zh="用户名" en="Username" />,
                Footer: () => <Trans zh="总计" en="Total" />,
                footerCellProps: {
                    textAlign: "center"
                },
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "liveDealerWinLossAmount",
                Header: (
                    <Trans zh="真人输赢金额" en="Live Dealer Win Loss Amount" />
                ),
                sumColumn: "liveDealerWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "sportsWinLossAmount",
                Header: <Trans zh="体育反水金额" en="Sports Win Loss Amount" />,
                sumColumn: "sportsWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "slotWinLossAmount",
                Header: <Trans zh="电子输赢金额" en="Slot Win Loss Amount" />,
                sumColumn: "slotWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "cardGameWinLossAmount",
                Header: (
                    <Trans zh="棋牌输赢金额" en="Card Game Win Loss Amount" />
                ),
                sumColumn: "cardGameWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "virtualSportsWinLossAmount",
                Header: (
                    <Trans
                        zh="虚拟体育输赢金额"
                        en="Virtual Sports Win Loss Amount"
                    />
                ),
                sumColumn: "virtualSportsWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "lotteryWinLossAmount",
                Header: (
                    <Trans
                        zh="彩票输赢金额"
                        en="Virtual Sports Win Loss Amount"
                    />
                ),
                sumColumn: "lotteryWinLossAmount",
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
}
