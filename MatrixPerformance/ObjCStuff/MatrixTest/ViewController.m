//
//  ViewController.m
//  MatrixTest
//
//  Created by Bill Holmes on 10/17/13.
//  Copyright (c) 2013 Xamarin. All rights reserved.
//

#import "ViewController.h"
#import "../MatrixtestLib/MatrixTestLib/TestClass.h"

@interface ViewController ()
@property (weak, nonatomic) IBOutlet UILabel *matrixSizeLabel;
- (IBAction)matrixSizeChanged:(id)sender;
@property (weak, nonatomic) IBOutlet UIStepper *matrixSizeStepper;
@property (weak, nonatomic) IBOutlet UIButton *runButton;
@property (weak, nonatomic) IBOutlet UILabel *resultLabel;

@end

@implementation ViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
	
    [self updateMatrixSizeLabel];
}

- (void)didReceiveMemoryWarning
{
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(int) getMatrixSize
{
    int count = (int)self.matrixSizeStepper.value;
    
    return (int)pow(2, count+8);
}

-(void) updateMatrixSizeLabel
{
    self.matrixSizeLabel.text = [NSString stringWithFormat:@"Matrix Size : %d", [self getMatrixSize]];
}

- (IBAction)matrixSizeChanged:(id)sender
{
    [self updateMatrixSizeLabel];
}

- (IBAction)runTest:(id)sender
{
    self.runButton.enabled = false;
    self.matrixSizeStepper.enabled = false;
    
    TestClass* t = [[TestClass alloc]init];
    t.matrixSize = [self getMatrixSize];
    [t runTest];
    
    self.resultLabel.text = [NSString stringWithFormat:@"Size %d ran in %.3f seconds", t.matrixSize, t.matrixMultiplyTime];

    self.matrixSizeStepper.enabled = true;
    self.runButton.enabled = true;
}

@end
