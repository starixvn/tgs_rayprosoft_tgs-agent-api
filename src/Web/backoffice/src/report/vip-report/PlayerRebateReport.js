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
import { usePlayerVipRebateList } from "./client"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import dateTimeColumn from "../../table/column/dateTime"
import {
    numberDecimalColumn,
    numberDecimalPercentageColumn
} from "../../table/column/numberColumn"

export const PlayerRebatDialogDisclosure = ({
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
                    start={start}
                    end={end}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const ReportDialog = ({ brandId, username, start, end, record, dialog }) => {
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
    } = usePlayerVipRebateList(username, toApiParams(state))

    let title = useTrans({
        zh: "返水详情",
        en: "Rebate Detail"
    })

    let { pageIndex, pageSize } = state
    let columns = usePlayerVipRebateColumns({ items: data?.items })

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

const usePlayerVipRebateColumns = ({ items = [] }) => {
    return useMemo(
        () => [
            dateTimeColumn({
                accessor: "reportDate",
                dateFormat: "yyyy/MM/dd",
                timeFormat: false,
                Header: <Trans zh="日期" en="Date" />,
                Footer: () => <Trans zh="总计" en="Total" />,
                footerCellProps: {
                    textAlign: "center"
                },
                disableSortBy: true
            }),
            numberDecimalPercentageColumn({
                accessor: "liveDealerRebate",
                Header: <Trans zh="真人反水比例" en="Live Dealer Rebate %" />,
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "liveDealerValidBetAmount",
                Header: (
                    <Trans
                        zh="真人投注金额"
                        en="Live Dealer Valid Bet Amount"
                    />
                ),
                sumColumn: "liveDealerValidBetAmount",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "liveDealerAmount",
                Header: (
                    <Trans zh="真人反水金额" en="Live Dealer Rebate Amount" />
                ),
                sumColumn: "liveDealerAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "sportsRebate",
                Header: <Trans zh="体育反水比例" en="Sports Rebate %" />,
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "sportsValidBetAmount",
                Header: (
                    <Trans zh="体育投注金额" en="Sports Valid Bet Amount" />
                ),
                sumColumn: "sportsValidBetAmount",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "sportsAmount",
                Header: <Trans zh="体育反水金额" en="Sports Rebate Amount" />,
                sumColumn: "sportsAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "slotRebate",
                Header: <Trans zh="电子反水比例" en="Slot Rebate %" />,
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "slotValidBetAmount",
                Header: <Trans zh="电子投注金额" en="Slot Valid Bet Amount" />,
                sumColumn: "slotValidBetAmount",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "slotAmount",
                Header: <Trans zh="电子反水金额" en="Slot Rebate Amount" />,
                sumColumn: "slotAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "cardGameRebate",
                Header: <Trans zh="棋牌反水比例" en="Card Game Rebate %" />,
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "cardGameValidBetAmount",
                Header: (
                    <Trans zh="棋牌投注金额" en="Card Game Valid Bet Amount" />
                ),
                sumColumn: "cardGameValidBetAmount",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "cardGameAmount",
                Header: (
                    <Trans zh="棋牌反水金额" en="Card Game Rebate Amount" />
                ),
                sumColumn: "cardGameAmount",
                disableSortBy: true
            }),

            numberDecimalPercentageColumn({
                accessor: "virtualSportsRebate",
                Header: (
                    <Trans zh="虚体反水比例" en="Virtual Sports Rebate %" />
                ),
                Footer: () => <Box />,
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "virtualSportsValidBetAmount",
                Header: (
                    <Trans
                        zh="虚体投注金额"
                        en="Virtual Sports Valid Bet Amount"
                    />
                ),
                sumColumn: "virtualSportsValidBetAmount",
                disableSortBy: true
            }),
            numberDecimalColumn({
                accessor: "virtualSportsAmount",
                Header: (
                    <Trans
                        zh="虚体反水金额"
                        en="Virtual Sports Rebate Amount"
                    />
                ),
                sumColumn: "virtualSportsAmount",
                disableSortBy: true
            })
        ],
        [JSON.stringify(items)]
    )
}
