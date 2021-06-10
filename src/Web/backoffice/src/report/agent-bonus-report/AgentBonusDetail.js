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
import {
    FormattedDateTime,
    FormattedNumberDecimal,
    Trans,
    useTrans
} from "../../i18n"
import {
    CloseButtonFooter,
    ConfirmButtonFooter
} from "../../form/ConfirmButtonFooter"
import { Table, toApiParams, useTable, pageCount } from "../../table"
import { useAgentDividendDetailList } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import dateTimeColumn from "../../table/column/dateTime"
import {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"
import AlertDialog from "../../dialog/Alert"
import textColumn from "../../table/column/textColumn"
import { formatDateTime } from "../../date/format"
import { FieldValues } from "../../layout/Grid"
import { formatNumberDecimal } from "../../number/format"
import Separator from "../../layout/Separator"
import { startOfNextDay } from "../../date/startOfNextDay"

export const AgentBonusDetailDialogDisclosure = ({
    brandId,
    agentId,
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
                <FormattedNumberDecimal value={value} />{" "}
            </Disclosure>
            {dialog.visible && (
                <ReportDialog
                    brandId={brandId}
                    agentId={agentId}
                    username={username}
                    bonusAmount={value}
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
    agentId,
    username,
    bonusAmount,
    start,
    end,
    dialog
}) => {
    let { state, setPageIndex, setSortOrder } = useTable({
        brandId,
        agentId,
        showAll: true
    })

    let { data, isLoading, error, mutate, deleteCache } =
        useAgentDividendDetailList({
            ...toApiParams(state),
            sortby: "username",
            from: start,
            to: startOfNextDay(end)
        })

    let { pageIndex, pageSize } = state
    let columns = useAgentBonusColumns({ username, items: data?.items })

    let title = useTrans({
        zh: "分红详情",
        en: "Profit Sharing Detail"
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
            <FieldStack>
                <FieldValues marginLeft="1rem">
                    <Trans zh="代理用户名" en="Agent Username" />
                    <Text.Block alignY="center">{username}</Text.Block>

                    <Trans zh="分红金额" en="Profit Sharing Amount" />
                    <Text.Block alignY="center">
                        {formatNumberDecimal(bonusAmount)}
                    </Text.Block>
                </FieldValues>
                <Separator />
                <Table
                    columns={columns}
                    data={data?.items}
                    initialState={state}
                    controlledState={state}
                    // pageCount={pageCount(pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    isLoading={isLoading}
                    foot={data?.items?.length > 0}
                />
            </FieldStack>
        </Dialog.Modal>
    )
}

const useAgentBonusColumns = ({ username = "", items = [] }) => {
    return useMemo(
        () => [
            textColumn({
                accessor: "username",
                Header: <Trans zh="代理用户名" en="Agent Username" />,
                Cell: ({ value }) => value ?? username
            }),
            dateTimeColumn({
                accessor: "settlementDate",
                dateFormat: "dd/MM/yyyy",
                timeFormat: false,
                Header: <Trans zh="结算日期" en="Date" />,
                Footer: () => <Trans zh="总计" en="Total" />,
                footerCellProps: {
                    textAlign: "center"
                },
                disableSortBy: true
            }),
            textColumn({
                id: "settlementInterval",
                accessor: (row) => row,
                Header: <Trans zh="结算区间" en="Settlement Interval" />,
                disableSortBy: true,
                Cell: ({ value }) => {
                    let format = "dd/MM/yyyy"
                    let start = formatDateTime(
                        format,
                        value?.settlementDateFrom
                    )
                    let end = formatDateTime(format, value?.settlementDateTo)
                    return <Text.Block>{`${start} - ${end}`}</Text.Block>
                }
            }),

            numberDecimalColumn({
                accessor: "liveDealerWinLoss",
                Header: (
                    <Trans
                        zh="真人输赢总金额"
                        en="Live Dealer Win Loss Amount"
                    />
                ),
                sumColumn: "liveDealerWinLoss",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "sportsWinLoss",
                Header: (
                    <Trans zh="体育输赢总金额" en="Sports Win Loss Amount" />
                ),
                sumColumn: "sportsWinLoss",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "slotWinLoss",
                Header: (
                    <Trans zh="电子输赢总金额" en="EGame Win Loss Amount" />
                ),
                sumColumn: "slotWinLoss",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "cardGameWinLoss",
                Header: (
                    <Trans zh="棋牌输赢总金额" en="Card Game Win Loss Amount" />
                ),
                sumColumn: "cardGameWinLoss",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "virtualSportsWinLoss",
                Header: (
                    <Trans
                        zh="虚拟体育输赢总金额"
                        en="Virtual Sports Win Loss Amount"
                    />
                ),
                sumColumn: "virtualSportsWinLoss",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "lotteryWinLoss",
                Header: (
                    <Trans zh="彩票输赢总金额" en="Lottery Win Loss Amount" />
                ),
                sumColumn: "lotteryWinLoss",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "agentDividendPercentage",
                Header: <Trans zh="分红比例" en="Profit Sharing Percentaget" />,
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "agentDividendAmount",
                Header: <Trans zh="分红金额" en="Profit Sharing Amount" />,
                sumColumn: "agentDividendAmount",
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
}
