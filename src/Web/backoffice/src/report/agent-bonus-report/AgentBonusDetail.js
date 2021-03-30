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
import { useAgentBonusDetailList } from "./client"
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

export const AgentBonusDetailDialogDisclosure = ({
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
                <FormattedNumberDecimal value={value} />{" "}
            </Disclosure>
            {dialog.visible && (
                <ReportDialog
                    brandId={brandId}
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
    username,
    bonusAmount,
    start,
    end,
    dialog
}) => {
    let { state, setPageIndex, setSortOrder } = useTable({
        brandId,
        username,
        from: start,
        to: end,
        showAll: true
    })
    let {
        data,
        isLoading,
        error,
        mutate,
        deleteCache
    } = useAgentBonusDetailList(username, toApiParams(state))

    let { pageIndex, pageSize } = state
    let columns = useAgentBonusColumns({ username, items: data?.items })

    let title = useTrans({
        zh: "分红详情",
        en: "Bonus Detail"
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

                    <Trans zh="分红金额" en="Bonus Amount" />
                    <Text.Block alignY="center">
                        {formatNumberDecimal(bonusAmount)}
                    </Text.Block>
                </FieldValues>
                <Separator />
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
                accessor: "reportDate",
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
                    let start = formatDateTime(format, value?.from)
                    let end = formatDateTime(format, value?.to)
                    return <Text.Block>{`${start} - ${end}`}</Text.Block>
                }
            }),

            numberDecimalColumn({
                accessor: "liveDealerWinLossAmount",
                Header: (
                    <Trans
                        zh="真人输赢总金额"
                        en="Live Dealer Win Loss Amount"
                    />
                ),
                sumColumn: "liveDealerWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "sportsWinLossAmount",
                Header: (
                    <Trans zh="体育输赢总金额" en="Sports Win Loss Amount" />
                ),
                sumColumn: "sportsWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "slotWinLossAmount",
                Header: <Trans zh="电子输赢总金额" en="Slot Win Loss Amount" />,
                sumColumn: "slotWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "cardGameWinLossAmount",
                Header: (
                    <Trans zh="棋牌输赢总金额" en="Card Game Win Loss Amount" />
                ),
                sumColumn: "cardGameWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "virtualSportsWinLossAmount",
                Header: (
                    <Trans
                        zh="虚拟体育输赢总金额"
                        en="Virtual Sports Win Loss Amount"
                    />
                ),
                sumColumn: "virtualSportsWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "lotteryWinLossAmount",
                Header: (
                    <Trans zh="彩票输赢总金额" en="Lottery Win Loss Amount" />
                ),
                sumColumn: "lotteryWinLossAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "bonusPercentage",
                Header: <Trans zh="分红比例" en="Bonus Percentaget" />,
                disableSortBy: true
            }),

            numberDecimalColumn({
                accessor: "bonusAmount",
                Header: <Trans zh="分红金额" en="Bonus Amount" />,
                sumColumn: "bonusAmount",
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
}
