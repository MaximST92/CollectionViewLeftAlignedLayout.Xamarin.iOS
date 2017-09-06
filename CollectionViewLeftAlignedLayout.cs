using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CollectionViewLeftAlignedLayout.Xamarin.iOS
{
    public static class CollectionViewLayoutAttributesExtension
    {
        public static void LeftAlignFrameWithSectionInset(this UICollectionViewLayoutAttributes layout, UIEdgeInsets insets)
        {
            var frame = layout.Frame;
            frame.X = insets.Left;
            layout.Frame = frame;
        }
    }
    
    public class CollectionViewLeftAlignedLayout: UICollectionViewFlowLayout
    {
        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            var listAttributes = new List<UICollectionViewLayoutAttributes>();
            var attributes = base.LayoutAttributesForElementsInRect(rect);
            if (attributes != null)
            {
                foreach (var layoutAttributese in attributes)
                {
                    if (layoutAttributese.RepresentedElementKind == null)
                    {
                        var index = layoutAttributese.IndexPath;
                        var attr = LayoutAttributesForItem(index);
                        if (attr != null)
                        {
                            layoutAttributese.Frame = attr.Frame;
                        }
                    }
                }
            }  
            return attributes;
        }
        

        public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath indexPath)
        {
            var currentItemAttributes = base.LayoutAttributesForItem(indexPath);
            var sectionInset = EvaluateMinimumInsetForItem(indexPath.Section);
            var isFirstItemInSection = indexPath.Item == 0;
            var layoutWidth = CollectionView.Frame.Width - sectionInset.Left - sectionInset.Right;

            if (isFirstItemInSection)
            {
                currentItemAttributes.LeftAlignFrameWithSectionInset(sectionInset);
                return currentItemAttributes;
            }

            var previousIndex = NSIndexPath.FromItemSection(indexPath.Item -1, indexPath.Section);
            var previousFrame = LayoutAttributesForItem(previousIndex)?.Frame ?? CGRect.Empty;
            var previousFrameRightPoint = previousFrame.X + previousFrame.Width;
            var currentFrame = currentItemAttributes.Frame;
            var stretchedCurrentFrame = new CGRect(sectionInset.Left,currentFrame.Y,layoutWidth,currentFrame.Height);

            var isFirstItemInRow = !previousFrame.IntersectsWith(stretchedCurrentFrame);

            if (isFirstItemInRow)
            {
                currentItemAttributes.LeftAlignFrameWithSectionInset(sectionInset);
                return currentItemAttributes;
            }

            var frame = currentItemAttributes.Frame;
            frame.X = previousFrameRightPoint + EvaluateMinimumInterItemSpacing(indexPath.Section);
            currentItemAttributes.Frame = frame;
            return currentItemAttributes;
        }

        private nfloat EvaluateMinimumInterItemSpacing(int index)
        {
            var del = CollectionView.Delegate as UICollectionViewDelegateFlowLayout;
            return del?.GetMinimumInteritemSpacingForSection(CollectionView,this,index) ?? MinimumInteritemSpacing;
        }
        
        private UIEdgeInsets EvaluateMinimumInsetForItem(int index)
        {
            var del = CollectionView.Delegate as UICollectionViewDelegateFlowLayout;
            return del?.GetInsetForSection(CollectionView,this,index) ?? SectionInset;
        }

    }
}